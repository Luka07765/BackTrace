using Trace.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Trace.Models.Auth;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Trace.Service.Token
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int TokenValidityDays = 7; // Base validity period for a new token

        public RefreshTokenService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Generate a new refresh token for a given user and invalidate all previously active tokens.
        /// </summary>
        public async Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress)
        {
            // Retrieve the user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new System.Collections.Generic.KeyNotFoundException("User not found.");
            }

            // Invalidate all active refresh tokens for the user before generating a new one
            await InvalidateAllUserRefreshTokens(userId, ipAddress);

            // Create a new refresh token
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

    
        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken == null)
            {
                return null;
            }

            // Basic validation checks
            if (refreshToken.Expires < DateTime.UtcNow || refreshToken.Revoked != null)
            {
                return null;
            }

            // Ensure that the user's session hasn't changed since the token was issued
            var user = refreshToken.User;
            if (user == null || refreshToken.UserId != user.Id || refreshToken.SessionVersion != user.SessionVersion)
            {
                return null;
            }

            return refreshToken;
        }

        /// <summary>
        /// Invalidate a specific refresh token and optionally link it to a new token.
        /// </summary>
        public async Task InvalidateRefreshToken(RefreshToken token, string ipAddress, string newToken = null)
        {
            token.Revoked = DateTime.UtcNow; // Mark as revoked
            token.RevokedByIp = ipAddress; // Log the revoking IP address
            token.ReplacedByToken = newToken; // Reference the new token if replacing

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Invalidate all active refresh tokens for a user.
        /// </summary>
        public async Task InvalidateAllUserRefreshTokens(string userId, string ipAddress)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.Revoked == null && t.Expires > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoked = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Exchange an existing valid refresh token for a new one (rotation).
        /// Implements sliding expiration by issuing a new token with a renewed expiration.
        /// </summary>
        public async Task<RefreshToken> RefreshAsync(string oldToken, string ipAddress)
        {
            var existingToken = await GetRefreshToken(oldToken);
            if (existingToken == null)
            {
                return null; // Invalid or expired token
            }

            var user = await _userManager.FindByIdAsync(existingToken.UserId);
            if (user == null)
            {
                // User no longer exists, revoke token
                await InvalidateRefreshToken(existingToken, ipAddress);
                return null;
            }

            // Invalidate the old token
            var newTokenString = GenerateTokenString();
            await InvalidateRefreshToken(existingToken, ipAddress, newTokenString);

            // Create a new rotated token with a "sliding" expiration
            var newRefreshToken = new RefreshToken
            {
                Token = newTokenString,
                Expires = DateTime.UtcNow.AddDays(TokenValidityDays),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserId = user.Id,
                SessionVersion = user.SessionVersion
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return newRefreshToken;
        }

        /// <summary>
        /// Helper method to generate a secure random token string.
        /// </summary>
        private string GenerateTokenString()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
