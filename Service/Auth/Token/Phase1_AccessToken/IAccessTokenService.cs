namespace Trace.Service.Auth.Token.Phase1_AccessToken
{
  
    using Trace.Models.Account;
    public interface IAccessTokenService
    {
        Task<string> CreateAccessToken(User user);
    }
}
