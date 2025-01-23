using Microsoft.AspNetCore.Identity;
using Trace.Models.Auth;
namespace Trace.Service.Auth.Token
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(ApplicationUser user);
        Task<TokenResponse> CreateTokenResponse(ApplicationUser user, string ipAddress);
        Task RevokeAccessToken(string token);
        Task<bool> IsAccessTokenRevoked(string token);
    }
}
