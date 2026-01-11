using Trace.DTO.Auth;
using Trace.Service.Auth.Token.Phase1_AccessToken;
using Trace.Service.Auth.Token.RefreshToken;
using Trace.Models.Account;
namespace Trace.Service.Auth.Token.Phase2_RefreshToken
{
    public class TokenResponseService : ITokenResponseService
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public TokenResponseService(
         IAccessTokenService accessTokenService,
         IRefreshTokenService refreshTokenService)
        {
            _accessTokenService = accessTokenService;
            _refreshTokenService = refreshTokenService;
        }


        public async Task<TokenResponse> CreateTokenResponse(User user, string ipAddress)
        {
            var accessToken = await _accessTokenService.CreateAccessToken(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, ipAddress);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
