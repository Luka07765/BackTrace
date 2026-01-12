using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using Trace.Models.Account;

namespace Trace.Service.Auth.Token.Phase3_Logout.InvalidateToken
{
    public class AccessInvalidationService : IAccessInvalidationService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<AccessInvalidationService> _logger;

        public AccessInvalidationService(IDistributedCache cache, ILogger<AccessInvalidationService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task RevokeAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse JWT token during revocation.");
                return;
            }

            var jti = jwtToken.Claims
                .FirstOrDefault(c => c.Type == CustomClaimTypes.Jti)?.Value;

            if (string.IsNullOrWhiteSpace(jti))
            {
                _logger.LogWarning("Custom JTI not found in token, unable to revoke.");
                return;
            }

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(jwtToken.ValidTo)
            };

            await _cache.SetStringAsync($"tokens:revoked:jti:{jti}", "true", options);
        }

        public async Task<bool> IsAccessTokenRevoked(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch
            {
                return true;
            }

            if (jwtToken.ValidTo < DateTime.UtcNow)
                return true;

            var jti = jwtToken.Claims
                .FirstOrDefault(c => c.Type == CustomClaimTypes.Jti)?.Value;

            if (string.IsNullOrWhiteSpace(jti))
                return true;

            var revoked = await _cache.GetStringAsync($"tokens:revoked:jti:{jti}");
            return !string.IsNullOrEmpty(revoked);
        }

        public bool ValidateSessionVersion(string token, User user)
        {
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch
            {
                return false;
            }

            var sessionVersionClaim = jwtToken.Claims
                .FirstOrDefault(c => c.Type == CustomClaimTypes.SessionVersion)?.Value;

            if (!int.TryParse(sessionVersionClaim, out var tokenSessionVersion))
                return false;

            return tokenSessionVersion == user.SessionVersion;
        }

        public async Task<bool> IsAccessTokenValid(string token, User user)
        {
            if (await IsAccessTokenRevoked(token))
                return false;

            if (!ValidateSessionVersion(token, user))
                return false;

            return true;
        }
    }
}
