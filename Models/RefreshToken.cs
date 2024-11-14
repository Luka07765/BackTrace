using Microsoft.AspNetCore.Identity;

namespace Trace.Models
{
    public class RefreshToken
    {
        public int Id { get; set; } // Primary Key
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; } // Made nullable
        public bool IsActive => Revoked == null && !IsExpired;

        public string UserId { get; set; } // Foreign Key
        public IdentityUser User { get; set; }
    }
}
