namespace Trace.Controllers.Auth
{
    using Microsoft.AspNetCore.Mvc;
    using Trace.Models.Account;
    using Microsoft.AspNetCore.Identity;
    using Trace.Service.Auth.Token.Phase1_AccessToken;
    using Trace.Service.Auth.Token.Phase4_Rotation;

    [ApiController]
    [Route("api/auth")]
    public class RefreshController : ControllerBase
    {
        private readonly ITokenRotationService _rotationService;
        private readonly IAccessTokenService _accessService;
        private readonly UserManager<User> _userManager;

        public RefreshController(
            ITokenRotationService rotationService,
            IAccessTokenService accessService,
            UserManager<User> userManager)
        {
            _rotationService = rotationService;
            _accessService = accessService;
            _userManager = userManager;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized();

            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized();

            var accessToken = authHeader["Bearer ".Length..].Trim();
            if (string.IsNullOrWhiteSpace(accessToken))
                return Unauthorized();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var newRefresh = await _rotationService.TokenRotation(refreshToken, accessToken, ip);
            if (newRefresh == null)
            {
                Response.Cookies.Delete("refreshToken");
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(newRefresh.UserId);
            if (user == null || user.SessionVersion != newRefresh.SessionVersion)
            {
                Response.Cookies.Delete("refreshToken");
                return Unauthorized();
            }

            var newAccess = await _accessService.CreateAccessToken(user);

            Response.Cookies.Append("refreshToken", newRefresh.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = newRefresh.Expires
            });

            return Ok(new { AccessToken = newAccess });
        }
    }
}
