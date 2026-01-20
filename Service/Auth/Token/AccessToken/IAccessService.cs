namespace Trace.Service.Auth.Token.AccessToken
{
    using Trace.Models.Account;
    public interface IAccessService
    {
        string CreateAccessToken(User user);
    }

}
