using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Trace.DTO.Auth;
using Trace.Models.Account;

namespace Trace.Service.Auth.GeneralAuth
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUser(RegisterDto model);
        Task<User> AuthenticateUser(LoginDto model);
    }
}