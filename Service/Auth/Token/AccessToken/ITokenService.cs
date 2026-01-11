using Microsoft.AspNetCore.Identity;
using Trace.DTO.Auth;
using Trace.Models.Account;

namespace Trace.Service.Auth.Token.AccessToken
{
    public interface ITokenService
    {
       
        Task RevokeAccessToken(string token);
        Task<bool> IsAccessTokenRevoked(string token);
    }
}
