
namespace Trace.Controllers.Auth
{

    using Microsoft.AspNetCore.Mvc;
    using Trace.Models.Account;

    using Trace.Service.Auth.Token.Phase1_AccessToken;
    using Trace.Service.Auth.Token.Phase4_Rotation;

    [ApiController]
    [Route("api/auth")]
    public class RefreshController : ControllerBase
    {
        private readonly ITokenRotationService _rotationService;
        private readonly IAccessTokenService _accessService;

        public RefreshController(
            ITokenRotationService rotationService,
            IAccessTokenService accessService)
        {
            _rotationService = rotationService;
            _accessService = accessService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            var newRefresh = await _rotationService.TokenRotation(refreshToken, ip);
            if (newRefresh == null)
            {
                Response.Cookies.Delete("refreshToken");
                return Unauthorized();
            }

            var userId = newRefresh.UserId;

            // Access token needs user — get minimal user object
            var user = new User
            {
                Id = userId,
                SessionVersion = newRefresh.SessionVersion
            };

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
