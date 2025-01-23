using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Trace.DTO;
using Trace.Models.Auth;
using Trace.Service.Auth.GeneralAuth;
using Trace.Service.Token;

namespace Jade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger; // Use ILogger<AuthController> for type-safe logging

        public AuthController(
            ITokenService tokenService,
            IUserService userService,
            IRefreshTokenService refreshTokenService,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthController> logger) // Add logger here
        {
            _tokenService = tokenService;
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _userManager = userManager;
            _logger = logger; // Assign logger
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterUser(model);
            if (result.Succeeded)
                return Ok("User registered successfully!");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt with email: {Email}", model.Email);
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid login attempt with email: {Email}", model.Email);
                return Unauthorized(new { message = "Invalid password." });
            }

            var ipAddress = GetIpAddress();

            // Generate tokens
            var tokenResponse = await _tokenService.CreateTokenResponse(user, ipAddress);

            // Set refresh token in HttpOnly cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure this is true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Set appropriate expiration
            };
            Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, cookieOptions);

            // Return only the access token in the response body
            return Ok(new { AccessToken = tokenResponse.AccessToken });
        }






        [Authorize]
        [HttpGet("ValidateToken")]
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                var userId = User.FindFirstValue("CustomUserId"); // Access custom claim "CustomUserId"
                var sessionVersionInToken = User.FindFirstValue("CustomSessionVersion");

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(sessionVersionInToken))
                {
                    return Unauthorized(new { message = "Token is invalid or missing claims." });
                }

                // Retrieve user from database by ID
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // Compare session versions
                if (user.SessionVersion.ToString() != sessionVersionInToken)
                {
                    return Unauthorized(new { message = "User has logged off. Token is invalid." });
                }

                // Token is valid
                return Ok(new { message = "Token is valid.", userId });
            }
            catch (Exception ex)
            {
                // Catch any unexpected errors
                return StatusCode(500, new { message = "An error occurred while validating the token.", error = ex.Message });
            }
        }



        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            // Retrieve the refresh token from the HttpOnly cookie
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required." });
            }

            var ipAddress = GetIpAddress();

            // Validate the refresh token
            var existingToken = await _refreshTokenService.GetRefreshToken(refreshToken);
            if (existingToken == null || !existingToken.IsActive)
            {
                return Unauthorized(new { message = "Refresh token is invalid or has been revoked." });
            }

            // Retrieve the associated user
            var user = await _userManager.FindByIdAsync(existingToken.UserId);
            if (user == null)
            {
                _logger.LogWarning($"User not found for UserId: {existingToken.UserId}");
                return Unauthorized(new { message = "User not found." });
            }

            // Validate session version
            if (user.SessionVersion != existingToken.SessionVersion)
            {
                return Unauthorized(new { message = "Invalid refresh token. User has logged off or session has expired." });
            }

            // Generate a new refresh token and invalidate the old one
            var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, ipAddress);
            await _refreshTokenService.InvalidateRefreshToken(existingToken, ipAddress, newRefreshToken.Token);

            // Generate a new access token
            var newAccessToken = await _tokenService.CreateAccessToken(user);

            // Set the new refresh token in HttpOnly cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure this is true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Set appropriate expiration
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            // Return the new access token
            return Ok(new { AccessToken = newAccessToken });
        }






        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue("CustomUserId"); // Access custom claim "CustomUserId"
            _logger.LogInformation($"Extracted userId: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User ID is null or empty.");
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var user = await _userManager.FindByIdAsync(userId); // Use FindByIdAsync for ID-based lookup
            _logger.LogInformation($"Found user: {user?.UserName}");

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Increment session version to invalidate all tokens
            user.SessionVersion++;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to update session version for user ID: {UserId}. Errors: {Errors}", userId, errors);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
            _logger.LogInformation("Session version incremented for user ID: {UserId}", userId);

            // Invalidate all refresh tokens
            var ipAddress = GetIpAddress();
            await _refreshTokenService.InvalidateAllUserRefreshTokens(userId, ipAddress);


            if (Request.Cookies.ContainsKey("refreshToken"))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Ensure this matches the setting used when setting the cookie
                    SameSite = SameSiteMode.Strict, // Ensure this matches the setting used when setting the cookie
                    Path = "/", // Ensure this matches the path used when setting the cookie
                    Expires = DateTime.UtcNow.AddDays(-1) // Expire the cookie immediately
                };
                Response.Cookies.Delete("refreshToken", cookieOptions);
                _logger.LogInformation("Refresh token cookie deleted for user ID: {UserId}", userId);
            }
        

            return Ok(new { message = "Logged out successfully." });
        }
  



        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
