namespace Trace.Service.Auth.Token.Phase3_Logout.InvalidateToken
{
    using Trace.Models.Account;

    public interface ITokenInvalidationService
    {
        Task RevokeAccessToken(string token);
        Task<bool> IsAccessTokenRevoked(string token);
        bool ValidateSessionVersion(string token, User user);
        Task<bool> IsAccessTokenValid(string token, User user);
    }
}
