using Trace.DTO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Trace.Models.Auth;

namespace Trace.Service.Auth
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUser(RegisterModel model);
        Task<ApplicationUser> AuthenticateUser(LoginModel model);
    }
}