using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using Trace.Data;
using Trace.Models.Logic;
using Trace.Service.Auth.Token;

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

            await _context.SaveChangesAsync();

            return Ok(new { message = "Share revoked" });
        }
        //comments
        [Authorize]
        [HttpPost("{fileId:guid}")]
        public async Task<IActionResult> CreateShare(Guid fileId)
        {

            var userId = User.FindFirstValue(CustomClaimTypes.UserId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var file = await _context.Files
                      .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);

            if (file == null)
                return NotFound(); 


            file.IsShared = true;
            file.ShareToken ??= Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            await _context.SaveChangesAsync();

            return Ok(new
            {
                shareUrl = $"{Request.Scheme}://{Request.Host}/api/share/public/{file.ShareToken}"
            });
        }

        // Public read-only access
        [AllowAnonymous]
        [HttpGet("public/{token}")]
        public async Task<IActionResult> GetSharedFile(string token)
        {
            var file = await _context.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(f =>
                    f.IsShared &&
                    f.ShareToken == token);

            if (file == null)
                return NotFound();

            return Ok(new
            {
                file.Title,
                file.Content
            });
        }
    }
}
