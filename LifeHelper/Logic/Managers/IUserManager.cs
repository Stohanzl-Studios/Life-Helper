using Blazored.LocalStorage;
using Blazored.SessionStorage;
using LifeHelper.Delegates;
using LifeHelper.Logic.Enums;
using SharedResources.Interfaces;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;

namespace LifeHelper.Logic.Managers
{
    public interface IUserManager
    {
        IUser CurrentUser { get; }
        UserState UserState { get; }

        OnUserStateChanged? OnUserStateChanged { get; set; }
        OnUserUpdated? OnUserUpdated { get; set; }

        OnUserLoggedIn? OnUserLoggedIn { get; set; }
        OnUserRegistered? OnUserRegistered { get; set; }
        OnTokensRefreshed? OnTokensRefreshed { get; set; }

        bool Login(ILoginRequest request, OnUserLoggedIn callback, ILocalStorageService localService, bool remember);
        bool Logout(OnUserStateChanged callback, ILocalStorageService localStorageService);
        Task PersistentLogin(ILocalStorageService localStorage, OnUserLoggedIn callback);
        Task CheckPersistent(ILocalStorageService localStorage, OnJWTTokenChecked callback);


        bool Register(IRegisterRequest request, OnUserRegistered callback, ILocalStorageService localService, bool remember);
        bool RefreshTokens(IRefreshTokenRequest request, OnTokensRefreshed callback);


        bool GetUser(IGetUserRequest request);
        bool UserExists(IUserExistsRequest request);

        bool UpdateUser(OnUserUpdated callback);
    }
}
