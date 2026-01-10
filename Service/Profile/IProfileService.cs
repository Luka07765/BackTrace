using Trace.Models.Account;

namespace Trace.Service.Profile
{
    public interface IProfileService
    {
        Task<string> UploadAvatarAsync(User user, IFormFile file);
        Task RemoveAvatarAsync(User user);
        Task ChangePasswordAsync(
         User user,
         string currentPassword,
         string newPassword);
    }
}

