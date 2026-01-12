namespace Trace.Service.Auth.Token.Phase4_Rotation
{
    using Microsoft.AspNetCore.Identity;
    using Trace.Models.Account;
    using Trace.Models.Auth;
    using Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh;
    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;
    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateToken;

    public class TokenRotationService : ITokenRotationService
    {
        private readonly ITokenRefreshService _refreshService;
        private readonly IRefreshInvalidationService _refreshInvalidation;
        private readonly IAccessInvalidationService _accessInvalidation;
        private readonly UserManager<User> _userManager;

        public TokenRotationService(
            ITokenRefreshService refreshService,
            IRefreshInvalidationService refreshInvalidation,
            IAccessInvalidationService accessInvalidation,
            UserManager<User> userManager)
        {
            _refreshService = refreshService;
            _refreshInvalidation = refreshInvalidation;
            _accessInvalidation = accessInvalidation;
            _userManager = userManager;
        }

        public async Task<RefreshToken?> TokenRotation(string oldRefreshToken, string oldAccessToken, string ipAddress)
        {
            var existing = await _refreshService.GetRefreshToken(oldRefreshToken);
            if (existing == null)
                return null;

            var user = await _userManager.FindByIdAsync(existing.UserId);
            if (user == null || existing.SessionVersion != user.SessionVersion)
            {
                await _refreshInvalidation.InvalidateRefreshToken(existing, ipAddress);
                return null;
            }

            // 🔴 THIS IS THE CRITICAL FIX
            await _accessInvalidation.RevokeAccessToken(oldAccessToken);

            var newRefresh = await _refreshService.GenerateRefreshToken(user.Id, ipAddress);

            await _refreshInvalidation.InvalidateRefreshToken(
                existing,
                ipAddress,
                newRefresh.Token
            );

            return newRefresh;
        }
    }
}
