using Trace.Data;
using Trace.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Trace.Service
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserId = userId
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            return await _context.RefreshTokens.Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task InvalidateRefreshToken(RefreshToken token, string ipAddress, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReplacedByToken = replacedByToken;

            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateAllUserRefreshTokens(string userId, string ipAddress)
        {
            // Fetch tokens based on UserId, then move filtering to client-side with AsEnumerable
            var tokens = _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .AsEnumerable() // Switch to client-side evaluation
                .Where(rt => rt.IsActive) // Apply IsActive filter in memory
                .ToList();

            foreach (var token in tokens)
            {
                await InvalidateRefreshToken(token, ipAddress);
            }
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
