using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;

namespace WebAPI.Managers
{
    public interface IUserManager
    {
        IResponse Login(ILoginRequest request);
        IResponse Register(IRegisterRequest request);
        IResponse RefreshTokens(IRefreshTokenRequest request);
        IResponse GetUser(IGetUserRequest request);
        IResponse UserExists(IUserExistsRequest request);
        IResponse GenerateJWT(ITokenRequest request);
        IResponse AuthenticateJWT(IJWTLoginRequest request);
        IResponse CheckJWT(IJWTLoginRequest request);

        IUser? GetActiveUser(string accessToken);

        void RemoveActiveUser(string? uid);
    }
}
