using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Concurrent;
using Trace.Models.Auth;

namespace Trace.Service.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;

        // In-memory storage for revoked access tokens (replace with a database for production)
        private static readonly ConcurrentDictionary<string, bool> _revokedAccessTokens = new ConcurrentDictionary<string, bool>();

        public TokenService(IConfiguration configuration, IRefreshTokenService refreshTokenService)
        {
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<string> CreateAccessToken(ApplicationUser user)
        {
            // Generate a unique identifier for the token (jti claim)
            var jti = Guid.NewGuid().ToString();

            var claims = new[]
            {
                 new Claim("CustomJti", Guid.NewGuid().ToString()), // Custom unique token ID
                   new Claim("CustomEmail", user.Email),
                 new Claim("CustomSubject", user.UserName), // Custom subject claim for username
                 new Claim("CustomUserId", user.Id), // Custom claim for user ID
                 new Claim("CustomUserName", user.UserName), // Custom claim for username
                   new Claim("CustomSessionVersion", user.SessionVersion.ToString()),

            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(15), // Access token expiration
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<TokenResponse> CreateTokenResponse(ApplicationUser user, string ipAddress)
        {
            var accessToken = await CreateAccessToken(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, ipAddress);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        // Method to revoke an access token
        public async Task RevokeAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = null;

            // Read the JWT token to extract the jti claim
            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception)
            {
                // Handle invalid token format
                await Task.CompletedTask;
                return;
            }

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                _revokedAccessTokens[jti] = true;
            }

            await Task.CompletedTask;
        }

        // Method to check if an access token has been revoked
        public async Task<bool> IsAccessTokenRevoked(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = null;

            // Read the JWT token to extract the jti claim
            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception)
            {
                // Token is invalid, consider it revoked
                return await Task.FromResult(true);
            }

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                return await Task.FromResult(_revokedAccessTokens.ContainsKey(jti));
            }

            return await Task.FromResult(false);
        }
    }
}
