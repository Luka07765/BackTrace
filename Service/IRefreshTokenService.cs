using Trace.Models;

namespace Trace.Service
{

    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress);
        Task<RefreshToken> GetRefreshToken(string token);
        Task InvalidateRefreshToken(RefreshToken token, string ipAddress, string replacedByToken = null);
        Task InvalidateAllUserRefreshTokens(string userId, string ipAddress);
    }
}

