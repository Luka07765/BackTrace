

namespace Trace.Controllers.Auth
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    using Trace.Models.Account;

    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;

    [ApiController]
    [Route("api/auth")]
    public class LogoutController : ControllerBase
    {
        private readonly IRefreshInvalidationService _invalidationService;
        private readonly UserManager<User> _userManager;

        public LogoutController(
            IRefreshInvalidationService invalidationService,
            UserManager<User> userManager)
        {
            _invalidationService = invalidationService;
            _userManager = userManager;
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue("CustomUserId");
            var user = await _userManager.FindByIdAsync(userId);

            user.SessionVersion++;
            await _userManager.UpdateAsync(user);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _invalidationService.InvalidateAllUserRefreshTokens(userId, ip);

            Response.Cookies.Delete("refreshToken");

            return Ok("Logged out.");
        }
    }
}
