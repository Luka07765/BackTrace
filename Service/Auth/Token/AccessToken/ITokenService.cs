using Microsoft.AspNetCore.Identity;
using Trace.DTO.Auth;
using Trace.Models.Account;

namespace Trace.Service.Auth.Token.AccessToken
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(User user);
        Task<TokenResponse> CreateTokenResponse(User user, string ipAddress);
        Task RevokeAccessToken(string token);
        Task<bool> IsAccessTokenRevoked(string token);
    }
}
