using Trace.Models.Auth;

namespace Trace.Service.Token
{

    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress);
        Task<RefreshToken> GetRefreshToken(string token);
        Task InvalidateRefreshToken(RefreshToken token, string ipAddress, string replacedByToken = null);
        Task InvalidateAllUserRefreshTokens(string userId, string ipAddress);
    }
}

