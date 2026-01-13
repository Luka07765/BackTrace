using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using Trace.Data;
using Trace.DTO;
using Trace.Service.Auth;

namespace Trace.Controllers
{
    [ApiController]
    [Route("api/share")]
    public class ShareController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShareController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpDelete("{fileId:guid}")]
        public async Task<IActionResult> RevokeShare(Guid fileId)
        {
            var userId = User.FindFirstValue(CustomClaimTypes.UserId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var file = await _context.Files
                .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);

            if (file == null)
                return NotFound();

            file.IsShared = false;
            file.ShareToken = null;
            file.ShareExpiresAt = null; // bug fix: clear expires too

            await _context.SaveChangesAsync();
            return Ok(new { message = "Share revoked" });
        }

        [Authorize]
        [HttpPost("{fileId:guid}")]
        public async Task<IActionResult> CreateShare(Guid fileId, [FromBody] CreateShareRequest? request)
        {
            var userId = User.FindFirstValue(CustomClaimTypes.UserId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var file = await _context.Files
                .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);

            if (file == null)
                return NotFound();

            var now = DateTime.UtcNow;

            // bug fix: if expired or missing token, generate a new one
            var isExpired = file.ShareExpiresAt != null && file.ShareExpiresAt < now;
            if (string.IsNullOrWhiteSpace(file.ShareToken) || isExpired)
            {
                file.ShareToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)); // uppercase hex
            }

            file.IsShared = true;
            file.ShareExpiresAt = now.AddDays(1); // keep your behavior

            await _context.SaveChangesAsync();

            var frontendBaseUrl = "https://front-w89v.vercel.app";
            return Ok(new
            {
                shareUrl = $"{frontendBaseUrl}/page/share/{file.ShareToken}",
                expiresAt = file.ShareExpiresAt
            });
        }

        [AllowAnonymous]
        [HttpGet("public/{token}")]
        public async Task<IActionResult> GetSharedFile(string token)
        {
            var normalizedToken = token.Trim().ToUpper();

            if (normalizedToken.Length == 0)
                return NotFound();

    
                var file = await _context.Files
        .FirstOrDefaultAsync(f =>
            f.IsShared &&
            f.ShareToken != null &&
            f.ShareToken.Equals(normalizedToken, StringComparison.OrdinalIgnoreCase));

            if (file == null)
                return NotFound();

            if (file.ShareExpiresAt != null && file.ShareExpiresAt < DateTime.UtcNow)
            {
    
                file.IsShared = false;
                file.ShareToken = null;
                file.ShareExpiresAt = null;

                await _context.SaveChangesAsync();
                return NotFound();
            }

            return Ok(new
            {
                file.Title,
                file.Content
            });
        }
    }
}
