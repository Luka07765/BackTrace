using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Trace.DTO.Auth;
using Trace.Models.Account;
using Trace.Service.Auth;
using Trace.Service.Auth.GeneralAuth;
using Trace.Service.Auth.Token.Phase1_AccessToken;
using Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh;
using Trace.Service.Auth.Token.Phase2_RefreshToken.Response;
using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;

namespace Jade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccessTokenService _tokenAccess;
        private readonly ITokenResponseService _tokenResponse;
        private readonly ITokenRefreshService _refreshService;
        private readonly IRefreshInvalidationService _invalidationService;
        private readonly IUserService _userService;
       
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger; // Use ILogger<AuthController> for type-safe logging

        public AuthController(
            IAccessTokenService accessTokenService,
            ITokenResponseService tokenResponseService,
            IUserService userService,
         
            ITokenRefreshService refreshService,
            IRefreshInvalidationService invalidationService,
            UserManager<User> userManager,
            ILogger<AuthController> logger) // Add logger here
        {
            _tokenAccess = accessTokenService;
            _tokenResponse = tokenResponseService;
            _refreshService = refreshService;
            _invalidationService = invalidationService;
            _userService = userService;
          
            _userManager = userManager;
            _logger = logger; 
        }







        [Authorize]
        [HttpGet("ValidateToken")]
        public async Task<IActionResult> ValidateToken()
        {
            var userId = User.FindFirstValue(CustomClaimTypes.UserId);
            var sessionVersionInToken = User.FindFirstValue(CustomClaimTypes.SessionVersion);

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(sessionVersionInToken))
                return Unauthorized(new { message = "Token missing required claims." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized(new { message = "User not found." });

            if (!int.TryParse(sessionVersionInToken, out var tokenSessionVersion))
                return Unauthorized(new { message = "Invalid session version." });

            if (user.SessionVersion != tokenSessionVersion)
                return Unauthorized(new { message = "Session expired." });

            return Ok(new { message = "Token is valid.", userId });
        }







        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
