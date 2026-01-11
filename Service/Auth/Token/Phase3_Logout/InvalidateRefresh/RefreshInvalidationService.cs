
namespace Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.Models.Auth;
    public class RefreshInvalidationService : IRefreshInvalidationService
    {
        private readonly ApplicationDbContext _context;

        public RefreshInvalidationService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task InvalidateRefreshToken(RefreshToken token, string ipAddress, string newToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReplacedByToken = newToken;

            await _context.SaveChangesAsync();
        }

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
    }
}
