using static SharedResources.Constants;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;
using Newtonsoft.Json;
using System.Net;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;
using LifeHelper.Logic;
using LifeHelper.Logic.Managers;
using LifeHelper.Logic.Enums;
using LifeHelper.Delegates;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using System.Security.Cryptography;
using System.Text;
using Blazored.SessionStorage;
using SharedResources.Interfaces;

namespace LifeHelper.Logic.Managers
{
    public class UserManager : IUserManager
    {
        private UserManager() { }
        public static UserManager Instance { get; } = new UserManager();

        public IUser CurrentUser { get; private set; } = Core.CreateUser();
        private CancellationTokenSource _UserCancelToken { get; set; } = new CancellationTokenSource();

        private UserState _UserState = UserState.LoggedOut;
        public UserState UserState
        {
            get { return _UserState; }
            private set
            {
                if (value != _UserState)
                    try
                    {
                        OnUserStateChanged.Invoke(value);
                    }
                    catch { }
                _UserState = value;
            }
        }

        public OnUserStateChanged? OnUserStateChanged { get; set; }
        public OnUserUpdated? OnUserUpdated { get; set; }

        public OnUserLoggedIn? OnUserLoggedIn { get; set; }
        public OnTokensRefreshed? OnTokensRefreshed { get; set; }
        public OnUserRegistered? OnUserRegistered { get; set; }


        public bool Register(IRegisterRequest request, OnUserRegistered callback, ILocalStorageService localStorageService, bool remember)
        {
            if (!request.IsValid()) return false;
            Core.HttpManager.PostObject(request, WebApiAddress + "/auth/registerWithCredentials", async data =>
            {
                if (data.Result == Result.Success)
                {
                    ITokenResponse response = (ITokenResponse)data.Response;
                    CurrentUser.AccessToken = response.AccessToken;
                    CurrentUser.RefreshToken = response.RefreshToken;
                    CurrentUser.SetRemainingActiveTime((int)response.ExpiresIn);
                    UserState = UserState.LoggedIn;
                    if (OnUserRegistered != null)
                        OnUserRegistered.Invoke(data);
                    UpdateUser(data =>
                    {
                        callback.Invoke(data);
                        if (data.Result == Result.Success)
                            UserState = UserState.Updated;
                    });
                    if (remember) await Core.HttpManager.PostObject((ITokenRequest)Core.CreateRequest<ITokenRequest>(CurrentUser.AccessToken), WebApiAddress + "/auth/generateJWT", async callback =>
                    {
                        if (callback.Result == Result.Success)
                        {
                            IJWTResponse response = (IJWTResponse)callback.Response;
                            await localStorageService.SetItemAsync("jwt-token", response.Token);
                        }
                    }, typeof(IJWTResponse), true);
                }
                else
                    callback.Invoke(data);
            }, typeof(ITokenResponse));
            return true;
        }
        public bool Login(ILoginRequest request, OnUserLoggedIn callback, ILocalStorageService localStorageService, bool remember)
        {
            if (!request.IsValid() || UserState != UserState.LoggedOut) return false;
            localStorageService.ContainKeyAsync("jwt-token").AsTask().ContinueWith(async exists =>
            {
                if (!exists.Result)
                {
                    await Core.HttpManager.PostObject(request, WebApiAddress + "/auth/loginWithCredentials", async data =>
                    {
                        if (data.Result == Result.Success)
                        {
                            ITokenResponse response = (ITokenResponse)data.Response;
                            CurrentUser.AccessToken = response.AccessToken;
                            CurrentUser.RefreshToken = response.RefreshToken;
                            CurrentUser.SetRemainingActiveTime(response.ExpiresIn ?? -1);
                            HandleUserTokens(_UserCancelToken.Token);
                            UserState = UserState.LoggedIn;
                            if (OnUserLoggedIn != null)
                                OnUserLoggedIn.Invoke(data);
                            UpdateUser(data =>
                            {
                                callback.Invoke(data);
                                if (data.Result == Result.Success)
                                    UserState = UserState.Updated;
                            });
                            if (remember) await Core.HttpManager.PostObject((ITokenRequest)Core.CreateRequest<ITokenRequest>(CurrentUser.AccessToken), WebApiAddress + "/auth/generateJWT", async callback =>
                            {
                                if (callback.Result == Result.Success)
                                {
                                    IJWTResponse response = (IJWTResponse)callback.Response;
                                    await localStorageService.SetItemAsync("jwt-token", response.Token);
                                }
                            }, typeof(IJWTResponse), true);
                        }
                        else
                        {
                            UserState = UserState.LoggedOut;
                            callback.Invoke(data);
                        }
                    }, typeof(ITokenResponse));
                }
                else if (CurrentUser == null || CurrentUser.AccessToken == null)
                    await PersistentLogin(localStorageService, callback);
            });
            return true;
        }
        public async Task PersistentLogin(ILocalStorageService localStorageService, OnUserLoggedIn callback)
        {
            if (!await localStorageService.ContainKeyAsync("jwt-token"))
            {
                try
                {
                    await localStorageService.RemoveItemAsync("jwt-token");
                    callback.Invoke(Core.CreateIDelegateBase(Result.InvalidJWTToken, "No user saved", "Please login with Remember Me"));
                    UserState = UserState.LoggedOut;
                }
                catch { }
                return;
            }
            IJWTLoginRequest request = (IJWTLoginRequest)Core.CreateRequest<IJWTLoginRequest>(await localStorageService.GetItemAsync<string>("jwt-token"));
            await Core.HttpManager.PostObject(request, WebApiAddress + "/auth/loginWithJWT", data =>
            {
                if (data.Result == Result.Success)
                {
                    ITokenResponse response = (ITokenResponse)data.Response;
                    CurrentUser.AccessToken = response.AccessToken;
                    CurrentUser.RefreshToken = response.RefreshToken;
                    CurrentUser.SetRemainingActiveTime(response.ExpiresIn ?? -1);
                    HandleUserTokens(_UserCancelToken.Token);
                    UserState = UserState.LoggedIn;
                    if (OnUserLoggedIn != null)
                        OnUserLoggedIn.Invoke(data);
                    UpdateUser(data =>
                    {
                        callback.Invoke(data);
                        if (data.Result == Result.Success)
                            UserState = UserState.Updated;
                    });
                    //IF DATE Expiring refresh
                    Core.HttpManager.PostObject((ITokenRequest)Core.CreateRequest<ITokenRequest>(CurrentUser.AccessToken), WebApiAddress + "/auth/generateJWT", async callback =>
                    {
                        if (callback.Result == Result.Success)
                        {
                            await localStorageService.RemoveItemAsync("jwt-token");
                            IJWTResponse response = (IJWTResponse)callback.Response;
                            await localStorageService.SetItemAsync("jwt-token", response.Token);
                        }
                    }, typeof(IJWTResponse), true);
                }
                else
                {
                    if (data.Result == Result.InvalidJWTToken)
                        localStorageService.RemoveItemAsync("jwt-token");
                    UserState = UserState.LoggedOut;
                    callback.Invoke(data);
                }
            }, typeof(ITokenResponse));
        }
        public async Task CheckPersistent(ILocalStorageService localStorage, OnJWTTokenChecked callback)
        {
            if (!await localStorage.ContainKeyAsync("jwt-token"))
            {
                callback.Invoke(false);
                return;
            }
            await Core.HttpManager.PostObject(Core.CreateRequest<IJWTLoginRequest>(await localStorage.GetItemAsync<string>("jwt-token")), WebApiAddress + "/auth/checkJWT", async data =>
            {
                if (data.Result == Result.Success)
                    callback.Invoke(true);
                else
                {
                    await localStorage.RemoveItemAsync("jwt-token");
                    callback.Invoke(false);
                }
            }, typeof(IJWTResponse));
        }
        public bool RefreshTokens(IRefreshTokenRequest request, OnTokensRefreshed callback)
        {
            if (!request.IsValid()) return false;
            Core.HttpManager.PostObject(request, WebApiAddress + "/auth/refreshAccessToken", data =>
            {
                if (data.Result == Result.Success)
                {
                    ITokenResponse response = (ITokenResponse)data.Response;
                    CurrentUser.AccessToken = response.AccessToken;
                    CurrentUser.RefreshToken = response.RefreshToken;
                    CurrentUser.SetRemainingActiveTime((int)response.ExpiresIn);
                    _UserCancelToken.Cancel();
                    _UserCancelToken = new CancellationTokenSource();
                    HandleUserTokens(_UserCancelToken.Token);
                    UserState = UserState.Refreshed;
                    if (OnTokensRefreshed != null)
                        OnTokensRefreshed.Invoke(data);
                }
                try
                {
                    callback.Invoke(data);
                }
                catch { }
            }, typeof(ITokenResponse), true);
            return true;
        }
        public bool Logout(OnUserStateChanged callback, ILocalStorageService localStorageService)
        {
            localStorageService.ClearAsync();
            CurrentUser = Core.CreateUser();
            if (callback != null)
                try { callback.Invoke(UserState.LoggedOut); } catch { }
            UserState = UserState.LoggedOut;
            return true;
        }

        public bool GetUser(IGetUserRequest request)
        {
            throw new NotImplementedException();
        }
        public bool UserExists(IUserExistsRequest request)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(OnUserUpdated callback)
        {
            Core.HttpManager.PostObject(Core.CreateRequest<IGetUserRequest>(null, CurrentUser.AccessToken), WebApiAddress + "/user", data =>
            {
                if (data.Result == Result.Success)
                {
                    IGetUserResponse response = (IGetUserResponse)data.Response;
                    if (!response.IsValid()) callback.Invoke(Core.CreateIDelegateBase(Result.Failure, "Response invalid", "Failed to update user info"));
                    else
                    {
                        response.User.AccessToken = CurrentUser.AccessToken;
                        response.User.RefreshToken = CurrentUser.RefreshToken;
                        response.User.ExpiresIn = CurrentUser.ExpiresIn;
                        CurrentUser = response.User;
                        callback.Invoke(Core.CreateIDelegateBase(Result.Success));
                        UserState = UserState.Updated;
                        if (OnUserUpdated != null)
                            OnUserUpdated.Invoke(data);
                    }
                }
                else callback.Invoke(Core.CreateIDelegateBase(data.Result, data.Error, data.Message));
            }, typeof(IGetUserResponse));
            return true;
        }

        private async void HandleUserTokens(CancellationToken token)
        {
            int half = (int)CurrentUser.GetRemainingActiveTime().TotalMilliseconds / 2;
            await Task.Delay(half);
            if (token.IsCancellationRequested) return;
            await Task.Delay(half - 5000);
            if (token.IsCancellationRequested) return;
            //TODO: Make the functions avare of user state. If access_token is invalid login|refresh & continue
            if (!RefreshTokens((IRefreshTokenRequest)Core.CreateRequest<IRefreshTokenRequest>(CurrentUser.RefreshToken), data =>
            {
                if (data.Result != Result.Success)
                { Console.WriteLine(data.Error); }//TODO: Show ERROR
            }))
            {

            }//TODO: Show ERROR
        }
    }
}
