

namespace Trace.Service.Auth.Token.AccessToken
{
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Trace.Models.Account;
    public class AccessService : IAccessService
    {
        private readonly IConfiguration _configuration;

        public AccessService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateAccessToken(User user)
        {
            string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? _configuration["Jwt:Key"]
                ?? throw new Exception("JWT secret is not set.");

            string validAudience = Environment.GetEnvironmentVariable("JWT_Audience")
                ?? _configuration["Jwt:Audience"];

            string validIssuer = Environment.GetEnvironmentVariable("JWT_Issuer")
                ?? _configuration["Jwt:Issuer"];

            string accessTokenLifetime = Environment.GetEnvironmentVariable("AccessTokenLifetime")
                ?? _configuration["Jwt:AccessTokenLifetime"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(accessTokenLifetime)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
