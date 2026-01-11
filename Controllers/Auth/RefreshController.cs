
namespace Trace.Controllers.Auth
{

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Trace.DTO.Auth;
    using Trace.Models.Account;

    using Trace.Service.Auth.Token.Phase1_AccessToken;
    using Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh;

    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;

    [ApiController]
    [Route("api/auth")]
    public class RefreshController : ControllerBase
    {
        private readonly ITokenRefreshService _refreshService;
        private readonly IRefreshInvalidationService _invalidationService;
        private readonly IAccessTokenService _accessService;
        private readonly UserManager<User> _userManager;

        public RefreshController(
            ITokenRefreshService refreshService,
            IRefreshInvalidationService invalidationService,
            IAccessTokenService accessService,
            UserManager<User> userManager)
        {
            _refreshService = refreshService;
            _invalidationService = invalidationService;
            _accessService = accessService;
            _userManager = userManager;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var existing = await _refreshService.GetRefreshToken(refreshToken);

            var user = await _userManager.FindByIdAsync(existing.UserId);

            var newRefresh = await _refreshService.GenerateRefreshToken(user.Id, ip);
            await _invalidationService.InvalidateRefreshToken(existing, ip, newRefresh.Token);

            var newAccess = await _accessService.CreateAccessToken(user);

            Response.Cookies.Append("refreshToken", newRefresh.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { AccessToken = newAccess });
        }
    }
}
