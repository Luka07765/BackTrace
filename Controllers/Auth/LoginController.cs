namespace Trace.Controllers.Auth
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Trace.DTO.Auth;
    using Trace.Models.Account;
    using Trace.Service.Auth.Token.Phase2_RefreshToken.Response;
    using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;

    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly ITokenResponseService _tokenResponse;
        private readonly IRefreshInvalidationService _refreshInvalidation;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            ITokenResponseService tokenResponse,
            IRefreshInvalidationService refreshInvalidation,
            UserManager<User> userManager,
            ILogger<LoginController> logger)
        {
            _tokenResponse = tokenResponse;
            _refreshInvalidation = refreshInvalidation;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            // 🔐 START NEW SESSION
            user.SessionVersion++;
            await _userManager.UpdateAsync(user);

            // 🔐 INVALIDATE ALL OLD REFRESH TOKENS
            await _refreshInvalidation.InvalidateAllRefreshTokens(user.Id, ip);

            // 🎟 ISSUE TOKENS FOR NEW SESSION
            var tokenResponse = await _tokenResponse.CreateTokenResponse(user, ip);

            Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { AccessToken = tokenResponse.AccessToken });
        }
    }
}
