using Microsoft.AspNetCore.Identity;

namespace Trace.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public int SessionVersion { get; set; } = 0;
        public string? ProfileImageUrl { get; set; }
    }
}
