using Trace.DTO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Trace.Models.Auth;

namespace Trace.Service.Auth
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
            // Check if the username already exists
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return IdentityResult.Failed(new IdentityError { Description = "Username already exists!" });

            // Check if the email already exists
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists!" });

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                SessionVersion = 0 // Add custom properties if required
            };

            return await _userManager.CreateAsync(user, model.Password);
        }


        public async Task<ApplicationUser> AuthenticateUser(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
                return null;

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            return result ? user : null;
        }
    }
}