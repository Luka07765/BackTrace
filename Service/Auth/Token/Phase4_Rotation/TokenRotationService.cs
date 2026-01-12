namespace Trace.Service.Auth.Token.Phase4_Rotation
{
    using Microsoft.AspNetCore.Identity;
    using Trace.Models.Account;
    using Trace.Models.Auth;
    using Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh;
    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;

    public class TokenRotationService : ITokenRotationService
    {
        private readonly ITokenRefreshService _refreshService;
        private readonly IRefreshInvalidationService _invalidationService;
        private readonly UserManager<User> _userManager;

        public TokenRotationService(
            ITokenRefreshService refreshService,
            IRefreshInvalidationService invalidationService,
            UserManager<User> userManager)
        {
            _refreshService = refreshService;
            _invalidationService = invalidationService;
            _userManager = userManager;
        }

        public async Task<RefreshToken?> TokenRotation(string oldToken, string ipAddress)
        {
            // Phase 2: load + validate
            var existing = await _refreshService.GetRefreshToken(oldToken);
            if (existing == null)
                return null;
            var user = await _userManager.FindByIdAsync(existing.UserId);
            if (user == null)
            {
                await _invalidationService.InvalidateRefreshToken(existing, ipAddress);
                return null;
            }

            // Replay protection
            if (existing.ReplacedByToken != null)
            {
                // BREACH RESPONSE: kill all tokens
                user.SessionVersion++;
                await _userManager.UpdateAsync(user);

                await _invalidationService.InvalidateAllUserRefreshTokens(user.Id, ipAddress);

                return null;
            }



            // Phase 2: generate new token
            var newRefresh = await _refreshService.GenerateRefreshToken(user.Id, ipAddress);

            // Phase 3: invalidate old token
            await _invalidationService.InvalidateRefreshToken(
                existing,
                ipAddress,
                newRefresh.Token
            );

            return newRefresh;
        }
    }
}
