
namespace Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh
{
    using Trace.Data;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Cryptography;
    using Trace.Models.Auth;
    using Microsoft.AspNetCore.Identity;
    using Trace.Models.Account;
    public class TokenRefreshService :ITokenRefreshService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private const int TokenValidityDays = 7;


        public TokenRefreshService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress)

        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var refreshToken = new RefreshToken
            {
                Token = GenerateTokenString(),
                Expires = DateTime.UtcNow.AddDays(TokenValidityDays),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserId = userId,
                SessionVersion = user.SessionVersion
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken?> GetRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .Where(rt => rt.Token == token)
                .OrderByDescending(rt => rt.Created)
                .FirstOrDefaultAsync();

            if (refreshToken == null)
                return null;

            if (refreshToken.Expires < DateTime.UtcNow ||
                refreshToken.Revoked != null ||
                refreshToken.ReplacedByToken != null)
                return null;

            var user = refreshToken.User;
            if (user == null ||
                refreshToken.UserId != user.Id ||
                refreshToken.SessionVersion != user.SessionVersion)
                return null;

            return refreshToken;
        }


        private string GenerateTokenString()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
