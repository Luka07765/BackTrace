using Trace.Models.Auth;

namespace Trace.Service.Profile
{
    public interface IProfileService
    {
        Task<string> UploadAvatarAsync(ApplicationUser user, IFormFile file);
        Task RemoveAvatarAsync(ApplicationUser user);
        Task ChangePasswordAsync(
         ApplicationUser user,
         string currentPassword,
         string newPassword);
    }
}

