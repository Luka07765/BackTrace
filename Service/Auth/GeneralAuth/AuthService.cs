using Trace.DTO;
using Microsoft.AspNetCore.Identity;
using Trace.Models.Auth;

namespace Trace.Service.Auth.GeneralAuth
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUser(RegisterModel model)
        {
            // Username must be unique
            if (await _userManager.FindByNameAsync(model.Username) != null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Username already exists." });
            }

     
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Email already exists." });
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                SessionVersion = 0
            };

            return await _userManager.CreateAsync(user, model.Password);
        }

        // ✅ FIXED: authenticate by EMAIL
        public async Task<ApplicationUser?> AuthenticateUser(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return null;

            var valid = await _userManager.CheckPasswordAsync(user, model.Password);
            return valid ? user : null;
        }
    }
}
