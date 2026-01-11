using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using Trace.Models.Account;

namespace Trace.Service.Auth.Token.Phase3_Logout
{
    public class TokenInvalidationService : ITokenInvalidationService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<TokenInvalidationService> _logger;

        public TokenInvalidationService(IDistributedCache cache, ILogger<TokenInvalidationService> logger)
        {
            _cache = cache;
            _logger = logger;
        }




        public async Task RevokeAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = null;

            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse JWT token during revocation.");
                return;
            }

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                _logger.LogWarning("JTI not found in token, unable to revoke.");
                return;
            }

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = jwtToken.ValidTo
            };

            await _cache.SetStringAsync($"tokens:revoked:jti:{jti}", "true", options);
            _logger.LogInformation("Token with JTI {JTI} has been revoked.", jti);
        }

        public async Task<bool> IsAccessTokenRevoked(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = null;

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

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (!string.IsNullOrEmpty(jti))
            {
                var revoked = await _cache.GetStringAsync($"tokens:revoked:jti:{jti}");
                return !string.IsNullOrEmpty(revoked);
            }

            return false;
        }

        public async Task<bool> ValidateSessionVersion(string token, User user)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = null;

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

            if (int.TryParse(sessionVersionClaim, out var tokenSessionVersion))
                return tokenSessionVersion == user.SessionVersion;

            return false;
        }

        public async Task<bool> IsAccessTokenValid(string token, User user)
        {
            if (await IsAccessTokenRevoked(token))
                return false;

            if (!await ValidateSessionVersion(token, user))
                return false;

            return true;
        }
    }
}
