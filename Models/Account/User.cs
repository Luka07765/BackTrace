using Microsoft.AspNetCore.Identity;

namespace Trace.Models.Account
{
    public class User : IdentityUser
    {
        public int SessionVersion { get; set; } = 0;
        public string? ProfileImageUrl { get; set; }
    }
}
