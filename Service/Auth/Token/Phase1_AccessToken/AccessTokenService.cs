
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Trace.Models.Account;


namespace Trace.Service.Auth.Token.Phase1_AccessToken
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly IConfiguration _configuration;


        public AccessTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateAccessToken(User user)
        {
            string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["Jwt:Key"];
            string validAudience = Environment.GetEnvironmentVariable("JWT_Audience") ?? _configuration["Jwt:Audience"];
            string validIssuer = Environment.GetEnvironmentVariable("JWT_Issuer") ?? _configuration["Jwt:Issuer"];
            string AccessTokenLifetime = Environment.GetEnvironmentVariable("AccessTokenLifetime") ?? _configuration["Jwt:AccessTokenLifetime"];

            var claims = new[]
            {
                new Claim(CustomClaimTypes.Jti, Guid.NewGuid().ToString()),
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
                expires: DateTime.UtcNow.AddMinutes(double.Parse(AccessTokenLifetime)),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
