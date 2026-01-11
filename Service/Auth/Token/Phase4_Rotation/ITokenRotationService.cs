namespace Trace.Service.Auth.Token.Phase4_Rotation
{
    using Trace.Models.Auth;
    public interface ITokenRotationService
    {
        Task<RefreshToken> RefreshAsync(string oldToken, string ipAddress);
    }
}
