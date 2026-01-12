namespace Trace.Service.Auth.Token.Phase3_Logout.InvalidateToken
{
    using System.Security.Claims;
    using Trace.Models.Account;

    public interface IAccessInvalidationService
    {
        Task RevokeAccessToken(string token);
        Task<bool> IsAccessTokenRevoked(string token);
        bool ValidateSessionVersion(ClaimsPrincipal principal, User user);

    }
}
