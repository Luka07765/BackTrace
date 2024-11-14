using Microsoft.AspNetCore.Identity;
using Trace.Models;
namespace Trace.Service
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(IdentityUser user);
        Task<TokenResponse> CreateTokenResponse(IdentityUser user, string ipAddress);
    }
}
