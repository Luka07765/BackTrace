using Trace.DTO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Trace.Service
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUser(RegisterModel model);
        Task<IdentityUser> AuthenticateUser(LoginModel model);
    }
}