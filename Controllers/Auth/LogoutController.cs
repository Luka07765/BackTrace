namespace Trace.Controllers.Auth
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using Trace.Models.Account;
    using Trace.Service.Auth;
    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;
    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateToken;

    [ApiController]
    [Route("api/auth")]
    public class LogoutController : ControllerBase
    {
        private readonly IRefreshInvalidationService _refreshInvalidation;
        private readonly IAccessInvalidationService _tokenInvalidation;
        private readonly UserManager<User> _userManager;

        public LogoutController(
            IRefreshInvalidationService refreshInvalidation,
            IAccessInvalidationService tokenInvalidation,
            UserManager<User> userManager)
        {
            _refreshInvalidation = refreshInvalidation;
            _tokenInvalidation = tokenInvalidation;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(CustomClaimTypes.UserId);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            // 1. Kill all sessions
            user.SessionVersion++;
            await _userManager.UpdateAsync(user);

            // 2. Revoke all refresh tokens
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _refreshInvalidation.InvalidateAllRefreshTokens(userId, ip);

            // 3. Revoke current access token (JTI blacklist)
            var accessToken = HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            if (!string.IsNullOrWhiteSpace(accessToken))
                await _tokenInvalidation.RevokeAccessToken(accessToken);

            // 4. Delete refresh cookie
            Response.Cookies.Delete("refreshToken");

            return Ok("Logged out.");
        }
    }
}
