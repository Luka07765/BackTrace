using Trace.DTO.Auth;
using Trace.Models.Account;
using Trace.Service.Auth.Token.Phase1_AccessToken;
using Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh;

namespace Trace.Service.Auth.Token.Phase2_RefreshToken.Response
{
    public class TokenResponseService : ITokenResponseService
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly ITokenRefreshService _refreshService;

        public TokenResponseService(
         IAccessTokenService accessTokenService,
         ITokenRefreshService refreshService)
        {
            _accessTokenService = accessTokenService;
            _refreshService = refreshService;
        }


        public async Task<TokenResponse> CreateTokenResponse(User user, string ipAddress)
        {
            var accessToken = await _accessTokenService.CreateAccessToken(user);
            var refreshToken = await _refreshService.GenerateRefreshToken(user.Id, ipAddress);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
