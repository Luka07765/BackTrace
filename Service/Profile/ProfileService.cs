
using Microsoft.AspNetCore.Identity;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Supabase;
using Trace.Models.Auth;

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



        public async Task ChangePasswordAsync(
    ApplicationUser user,
    string currentPassword,
    string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword);

            if (!result.Succeeded)
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            // Invalidate all sessions
            user.SessionVersion++;
            await _userManager.UpdateAsync(user);
        }

        public async Task RemoveAvatarAsync(ApplicationUser user)
        {
            var bucket = _supabase.Storage.From("avatars");

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var oldPath = user.ProfileImageUrl
                    .Split("/avatars/")[1]
                    .Split("?")[0];

                await bucket.Remove(oldPath);
            }

            user.ProfileImageUrl = null;
            user.SessionVersion++;

            await _userManager.UpdateAsync(user);
        }


        public async Task<string> UploadAvatarAsync(ApplicationUser user, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file uploaded.");

            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("Max file size is 2MB.");

            var allowedTypes = new[]
            {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

            if (!allowedTypes.Contains(file.ContentType))
                throw new InvalidOperationException("Invalid image type.");

            using var inputStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            const int avatarSize = 512;

            image.Mutate(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(avatarSize, avatarSize),
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center
                });
            });

            byte[] webpBytes;
            using (var outStream = new MemoryStream())
            {
                var encoder = new WebpEncoder { Quality = 80 };
                await image.SaveAsync(outStream, encoder);
                webpBytes = outStream.ToArray();
            }

            var bucket = _supabase.Storage.From("avatars");

            // Delete old avatar file
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var oldPath = user.ProfileImageUrl
                    .Split("/avatars/")[1]
                    .Split("?")[0];

                await bucket.Remove(oldPath);
            }

            // New versioned filename
            var newVersion = user.SessionVersion + 1;
            var path = $"users/{user.Id}_v{newVersion}.webp";

            await bucket.Upload(
                webpBytes,
                path,
                new Supabase.Storage.FileOptions
                {
                    ContentType = "image/webp",
                    CacheControl = "0",
                    Upsert = true
                });

            user.SessionVersion = newVersion;
            user.ProfileImageUrl = bucket.GetPublicUrl(path);

            await _userManager.UpdateAsync(user);

            return user.ProfileImageUrl;
        }

    }
}