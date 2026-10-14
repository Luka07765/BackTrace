namespace Trace.GraphQL.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using HotChocolate.AspNetCore;
    using HotChocolate.AspNetCore.Subscriptions;
    using HotChocolate.AspNetCore.Subscriptions.Protocols;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.JsonWebTokens;

    public sealed class JwtWebSocketAuthInterceptor : DefaultSocketSessionInterceptor
    {
        private const string BearerPrefix = "Bearer ";
        private static readonly string[] TokenKeys = { "Authorization", "authToken", "token" };

        // Task.Delay can't wait much longer than this; longer-lived tokens aren't auto-closed.
        private static readonly TimeSpan MaxScheduledDelay = TimeSpan.FromDays(24);

        private readonly IOptionsMonitor<JwtBearerOptions> _jwtOptions;
        private readonly ILogger<JwtWebSocketAuthInterceptor> _logger;

        public JwtWebSocketAuthInterceptor(
            IOptionsMonitor<JwtBearerOptions> jwtOptions,
            ILogger<JwtWebSocketAuthInterceptor> logger)
        {
            _jwtOptions = jwtOptions;
            _logger = logger;
        }

        public override async ValueTask<ConnectionStatus> OnConnectAsync(
            ISocketSession session,
            IOperationMessagePayload payload,
            CancellationToken cancellationToken)
        {
            var token = ExtractToken(payload);
            if (token is null)
            {
                _logger.LogInformation("WebSocket connection rejected: no token in connectionParams.");
                return ConnectionStatus.Reject("Missing Authorization token.");
            }

            // Same settings and claim mapping as the HTTP JwtBearer pipeline.
            var options = _jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme);
            var handler = new JsonWebTokenHandler { MapInboundClaims = options.MapInboundClaims };

            var result = await handler.ValidateTokenAsync(token, options.TokenValidationParameters);
            if (!result.IsValid)
            {
                _logger.LogInformation(
                    "WebSocket connection rejected: invalid token ({Reason}).",
                    result.Exception?.GetType().Name ?? "unknown");
                return ConnectionStatus.Reject("Invalid token.");
            }

            session.Connection.HttpContext.User = new ClaimsPrincipal(result.ClaimsIdentity);

            ScheduleCloseOnExpiry(session, result.SecurityToken.ValidTo);

            return await base.OnConnectAsync(session, payload, cancellationToken);
        }

        private static string? ExtractToken(IOperationMessagePayload payload)
        {
            var values = payload.As<Dictionary<string, object?>>();
            if (values is null)
                return null;

            var raw = values
                .FirstOrDefault(kv => TokenKeys.Contains(kv.Key, StringComparer.OrdinalIgnoreCase))
                .Value?
                .ToString()?
                .Trim();

            if (string.IsNullOrEmpty(raw))
                return null;

            if (raw.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
                raw = raw.Substring(BearerPrefix.Length).Trim();

            return raw.Length == 0 ? null : raw;
        }

        // The token is only checked at connect time, so close the socket when it expires.
        private void ScheduleCloseOnExpiry(ISocketSession session, DateTime validToUtc)
        {
            if (validToUtc == DateTime.MinValue)
                return; // no exp claim

            var remaining = validToUtc - DateTime.UtcNow;
            if (remaining > MaxScheduledDelay)
                return;

            var connection = session.Connection;
            var connectionAborted = connection.HttpContext.RequestAborted;

            _ = Task.Run(async () =>
            {
                try
                {
                    if (remaining > TimeSpan.Zero)
                        await Task.Delay(remaining, connectionAborted);

                    if (!connection.IsClosed)
                    {
                        _logger.LogInformation("Closing WebSocket connection: token expired.");
                        await connection.CloseAsync(
                            "Token expired.",
                            ConnectionCloseReason.NormalClosure,
                            CancellationToken.None);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Connection closed before the token expired.
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to close WebSocket connection on token expiry.");
                }
            });
        }
    }
}