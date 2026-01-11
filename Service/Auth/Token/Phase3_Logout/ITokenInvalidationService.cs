namespace Trace.Service.Auth.Token.Phase3_Logout
{
    public interface ITokenInvalidationService
    {
        Task RevokeAccessToken(string token);
        Task<bool> IsAccessTokenRevoked(string token);
    }
}
