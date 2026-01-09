namespace Trace.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;
    using System.Security.Claims;
    using Trace.DTO;
    using Trace.Models.Auth;
    using Trace.Service.Auth.Token;
    using Trace.Service.Profile;

    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(
            IProfileService profileService,
            UserManager<ApplicationUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }



        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
    [FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(CustomClaimTypes.UserId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            try
            {
                await _profileService.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword);

                return Ok(new { message = "Password updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("avatar")]
        public async Task<IActionResult> RemoveAvatar()
        {
            var userId = User.FindFirstValue(CustomClaimTypes.UserId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            await _profileService.RemoveAvatarAsync(user);

            return Ok(new { message = "Avatar removed" });
        }

        [Authorize]
        [HttpPost("avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar(
      [FromForm] UploadAvatarRequest request,
      [FromServices] IDistributedCache cache)
        {
            var userId = User.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cacheKey = $"avatar-upload:{userId}";

            var countString = await cache.GetStringAsync(cacheKey);
            var count = string.IsNullOrEmpty(countString) ? 0 : int.Parse(countString);

            if (count >= 5)
                return StatusCode(429, "Too many avatar uploads. Try again later.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            var imageUrl = await _profileService.UploadAvatarAsync(user, request.File);

            await cache.SetStringAsync(
                cacheKey,
                (count + 1).ToString(),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });

            return Ok(new { imageUrl });
        }


    }
}