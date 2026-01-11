namespace Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh
{

    using Trace.Models.Auth;

    public interface ITokenRefreshService
    {
        Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress);
        Task<RefreshToken?> GetRefreshToken(string token);
    }
}
