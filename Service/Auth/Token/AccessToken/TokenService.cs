using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Trace.DTO.Auth;
using Trace.Models.Account;
using Trace.Service.Auth.Token.RefreshToken;
//start of auth 
namespace Trace.Service.Auth.Token.AccessToken
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IDistributedCache _cache;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IConfiguration configuration,
            IRefreshTokenService refreshTokenService,
            IDistributedCache cache,
            ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string> CreateAccessToken(User user)
        {
            string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["Jwt:Key"]
    ?? throw new Exception("JWT secret is not set.");
            string validAudience = Environment.GetEnvironmentVariable("JWT_Audience") ?? _configuration["Jwt:Audience"];
            string validIssuer = Environment.GetEnvironmentVariable("JWT_Issuer") ?? _configuration["Jwt:Issuer"];
            string AccessTokenLifetime = Environment.GetEnvironmentVariable("AccessTokenLifetime") ?? _configuration["Jwt:AccessTokenLifetime"];

            var jti = Guid.NewGuid().ToString();

            var claims = new[]
{
            new Claim(CustomClaimTypes.Jti, Guid.NewGuid().ToString()), // Unique token ID
            new Claim(CustomClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(CustomClaimTypes.Subject, user.UserName ?? string.Empty),
            new Claim(CustomClaimTypes.UserId, user.Id ?? string.Empty),
            new Claim(CustomClaimTypes.UserName, user.UserName ?? string.Empty),
            new Claim(CustomClaimTypes.SessionVersion, user.SessionVersion.ToString())
};


            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var token = new JwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(AccessTokenLifetime)), // Configurable expiration
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<TokenResponse> CreateTokenResponse(User user, string ipAddress)
        {
            var accessToken = await CreateAccessToken(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, ipAddress);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
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

            // Extract JTI
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                _logger.LogWarning("JTI not found in token, unable to revoke.");
                return;
            }

            // Set revoked flag in distributed cache
            var tokenExpiration = jwtToken.ValidTo; // Use token's expiration
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = tokenExpiration // Expire cache entry when the token itself expires
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse JWT token during revocation check.");
                return true; // Treat invalid token as revoked
            }

            // Check expiration
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                _logger.LogWarning("Token has expired.");
                return true;
            }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse JWT token during SessionVersion validation.");
                return false;
            }

            var sessionVersionClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.SessionVersion)?.Value;
            if (int.TryParse(sessionVersionClaim, out var tokenSessionVersion))
            {
                return tokenSessionVersion == user.SessionVersion;
            }

            _logger.LogWarning("Session version mismatch or missing in token.");
            return false;
        }

        public async Task<bool> IsAccessTokenValid(string token, User user)
        {
            if (await IsAccessTokenRevoked(token))
            {
                _logger.LogWarning("Access token is revoked.");
                return false;
            }

            if (!await ValidateSessionVersion(token, user))
            {
                _logger.LogWarning("Session version validation failed for token.");
                return false;
            }

            _logger.LogInformation("Access token is valid.");
            return true;
        }
    }
}
