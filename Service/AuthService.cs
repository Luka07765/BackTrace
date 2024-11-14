using Trace.DTO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Trace.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUser(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return IdentityResult.Failed(new IdentityError { Description = "User already exists!" });

            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<IdentityUser> AuthenticateUser(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return null;

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            return result ? user : null;
        }
    }
}