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
            if (token == null) return;

            var entity = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Id == token.Id);

            if (entity == null) return;
            if (entity.Revoked != null) return;

            var now = DateTime.UtcNow;

            entity.Revoked = now;
            entity.RevokedByIp = ipAddress;
            entity.ReplacedByToken = newToken;

            await _context.SaveChangesAsync();
        }


        public async Task InvalidateAllUserRefreshTokens(string userId, string ipAddress)
        {
            var now = DateTime.UtcNow;

            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.Revoked == null && t.Expires > now)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoked = now;
                token.RevokedByIp = ipAddress;
            }

            await _context.SaveChangesAsync();
        }
    }
}
