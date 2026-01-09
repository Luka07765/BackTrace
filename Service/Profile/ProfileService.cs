
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
        public async Task RemoveAvatarAsync(ApplicationUser user)
        {
            var path = $"users/{user.Id}.webp";
            var bucket = _supabase.Storage.From("avatars");

           
            await bucket.Remove(path);


            // Reset profile image
            user.ProfileImageUrl = null;

            // Increment version to invalidate caches
            user.SessionVersion++;

            await _userManager.UpdateAsync(user);
        }

        public async Task<string> UploadAvatarAsync(ApplicationUser user, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file uploaded.");

            // Size limit (2MB) – still keep this (prevents abuse)
            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("Max file size is 2MB.");

            // MIME allowlist (input types)
            var allowedTypes = new[]
            {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

            if (!allowedTypes.Contains(file.ContentType))
                throw new InvalidOperationException("Invalid image type.");

            // Load & validate image bytes (throws if not a real image)
            using var inputStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            // Resize + crop to square avatar (512x512)
            const int avatarSize = 512;

            image.Mutate(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(avatarSize, avatarSize),
                    Mode = ResizeMode.Crop,     // crop center to square
                    Position = AnchorPositionMode.Center
                });
            });

            // Encode to WebP (control output size/quality)
            byte[] webpBytes;
            using (var outStream = new MemoryStream())
            {
                var encoder = new WebpEncoder
                {
                    Quality = 80 // 0-100; 75-85 is a good avatar range
                };

                await image.SaveAsync(outStream, encoder);
                webpBytes = outStream.ToArray();
            }

            // Upload WebP to Supabase
            var path = $"users/{user.Id}.webp";
            var bucket = _supabase.Storage.From("avatars");

            await bucket.Upload(
      webpBytes,
      path,
      new Supabase.Storage.FileOptions
      {
          ContentType = "image/webp",
          Upsert = true,
          CacheControl = "0" 
      });


            user.SessionVersion++;

            var publicUrl = bucket.GetPublicUrl(path) + $"?v={user.SessionVersion}";
            user.ProfileImageUrl = publicUrl;


            await _userManager.UpdateAsync(user);

            return publicUrl;
        }

    }
}