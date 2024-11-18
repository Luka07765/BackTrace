using Trace.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Trace.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace Trace.Service.Token
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Generate a new refresh token
        public async Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress)
        {
            // Optionally revoke all active tokens for the user

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }


            // Create a new refresh token
            var refreshToken = new RefreshToken
            {
                Token = GenerateTokenString(),
                Expires = DateTime.UtcNow.AddDays(7), // Token valid for 7 days
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserId = userId,
                SessionVersion = user.SessionVersion 
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            await InvalidateAllUserRefreshTokens(userId, ipAddress);
            return refreshToken;
        }

        // Retrieve an existing refresh token
        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User) // Include User entity if needed
                .SingleOrDefaultAsync(rt => rt.Token == token);

            // Validate the token
            if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow || refreshToken.Revoked != null)
            {
                return null; // Return null for invalid or expired tokens
            }

            return refreshToken;
        }

        // Invalidate a specific refresh token
        public async Task InvalidateRefreshToken(RefreshToken token, string ipAddress, string newToken)
        {
            token.Revoked = DateTime.UtcNow; // Mark as revoked
            token.RevokedByIp = ipAddress; // Log the revoking IP address
            token.ReplacedByToken = newToken; // Reference the new token if replacing

            await _context.SaveChangesAsync(); // Save changes to the database
        }

        // Invalidate all active refresh tokens for a user
        public async Task InvalidateAllUserRefreshTokens(string userId, string ipAddress)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.Revoked == null && t.Expires > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoked = DateTime.UtcNow; // Mark as revoked
                token.RevokedByIp = ipAddress; // Log the revoking IP address
            }

            await _context.SaveChangesAsync(); // Save changes to the database
        }

        // Helper method to generate a secure random token string
        private string GenerateTokenString()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber); // Fill the array with secure random bytes
            return Convert.ToBase64String(randomNumber); // Convert to Base64 string
        }
    }
}

