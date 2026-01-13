namespace Trace.DTO
{
    public class UserProfile
    {
        public string Id { get; set; } = default!;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
