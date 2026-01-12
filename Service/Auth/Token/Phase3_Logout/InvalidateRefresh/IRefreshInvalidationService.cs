namespace Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh
{
    using Trace.Models.Auth;
    public interface IRefreshInvalidationService
    {
        Task InvalidateRefreshToken(RefreshToken token, string ipAddress, string replacedByToken = null);
        Task InvalidateAllRefreshTokens(string userId, string ipAddress);
    }
}
