using Trace.DTO.Auth;
using Trace.Models.Account;
namespace Trace.Service.Auth.Token.Phase2_RefreshToken.Response
{
    public interface ITokenResponseService
    {
        Task<TokenResponse> CreateTokenResponse(User user, string ipAddress);
    }
}
