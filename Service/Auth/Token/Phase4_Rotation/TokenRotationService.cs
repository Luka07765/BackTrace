
namespace Trace.Service.Auth.Token.Phase4_Rotation
{
    using Microsoft.AspNetCore.Identity;
    using Trace.Data;
    using Trace.Models.Account;
    using Trace.Models.Auth;
    using Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh;
    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;
    public class TokenRotationService : ITokenRotationService
    {
        private readonly ITokenRefreshService _refreshService;
        private readonly IRefreshInvalidationService _invalidationService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TokenRotationService(
            ITokenRefreshService refreshService,
            IRefreshInvalidationService invalidationService,
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _refreshService = refreshService;
            _invalidationService = invalidationService;
            _context = context;
            _userManager = userManager;
        }


        public async Task<RefreshToken> RefreshAsync(string oldToken, string ipAddress)
        {
            var existingToken = await _refreshService.GetRefreshToken(oldToken);
            if (existingToken == null)
                return null;

            var user = await _userManager.FindByIdAsync(existingToken.UserId);
            if (user == null)
            {
                await _invalidationService.InvalidateRefreshToken(existingToken, ipAddress);
                return null;
            }

            var newTokenString = Guid.NewGuid().ToString("N");

            await _invalidationService.InvalidateRefreshToken(existingToken, ipAddress, newTokenString);

            var newRefreshToken = new RefreshToken
            {
                Token = newTokenString,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserId = user.Id,
                SessionVersion = user.SessionVersion
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return newRefreshToken;
        }
    }
}
