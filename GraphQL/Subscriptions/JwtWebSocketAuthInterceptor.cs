

namespace Trace.GraphQL.Subscriptions
{
    using HotChocolate.AspNetCore;
    using HotChocolate.AspNetCore.Subscriptions;
    using HotChocolate.AspNetCore.Subscriptions.Protocols;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    public sealed class JwtWebSocketAuthInterceptor : DefaultSocketSessionInterceptor
    {
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtWebSocketAuthInterceptor(TokenValidationParameters tokenValidationParameters)
        {
            _tokenValidationParameters = tokenValidationParameters;
        }

        public override async ValueTask<ConnectionStatus> OnConnectAsync(
            ISocketSession session,
            IOperationMessagePayload payload,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("🧩 [JwtWebSocketAuthInterceptor] OnConnectAsync triggered");

            try
            {
                var payloadDict = payload.As<Dictionary<string, object>>();

                if (payloadDict == null || !payloadDict.TryGetValue("Authorization", out var authValue))
                {
                    Console.WriteLine("⚠️ No Authorization header found in connectionParams");
                    return ConnectionStatus.Reject("Missing Authorization");
                }

                var tokenString = authValue?.ToString()?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(tokenString))
                {
                    Console.WriteLine("⚠️ Empty token");
                    return ConnectionStatus.Reject("Empty token");
                }

                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(tokenString, _tokenValidationParameters, out var _);

                // ✅ Attach the user to the WebSocket session
                session.Connection.HttpContext.User = principal;

                Console.WriteLine($"✅ Authenticated WebSocket user: {principal.Identity?.Name ?? "Anonymous"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ WebSocket auth failed: {ex.Message}");
                return ConnectionStatus.Reject("Invalid token");
            }

            return await base.OnConnectAsync(session, payload, cancellationToken);
        }
    }
}
