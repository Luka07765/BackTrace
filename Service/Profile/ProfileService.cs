
using Microsoft.AspNetCore.Identity;
using Supabase;
using Trace.Models.Auth;
using SixLabors.ImageSharp;

namespace Trace.Service.Profile
{

    public class ProfileService : IProfileService
    {
        private readonly Client _supabase;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(
            Client supabase,
            UserManager<ApplicationUser> userManager)
        {
            _supabase = supabase;
            _userManager = userManager;
        }

        public async Task<string> UploadAvatarAsync(ApplicationUser user, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file uploaded.");

            // 1️⃣ Size limit (2MB)
            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("Max file size is 2MB.");

            // 2️⃣ MIME allowlist
            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(file.ContentType))
                throw new InvalidOperationException("Invalid image type.");

            // 3️⃣ Validate image bytes
            using var image = Image.Load(file.OpenReadStream());

            // 4️⃣ Convert file → byte[]
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

        
            var path = $"users/{user.Id}.jpg";
            var bucket = _supabase.Storage.From("avatars");

            await bucket.Upload(
                fileBytes,
                path,
                new Supabase.Storage.FileOptions
                {
                    ContentType = file.ContentType,
                    Upsert = true
                });


            var publicUrl = bucket.GetPublicUrl(path);
            user.ProfileImageUrl = publicUrl;

            await _userManager.UpdateAsync(user);

            return publicUrl;
        }
    }
}
