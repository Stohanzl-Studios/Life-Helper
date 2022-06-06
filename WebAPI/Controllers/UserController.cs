using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedResources.Enums;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;

namespace WebAPI.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [Route("auth/loginWithCredentials")]
        public async Task<string> LoginWithCredentials()
        {
            ILoginRequest? loginRequest = await Core.GetFromBody<ILoginRequest>(Request.Body);
            if (loginRequest == null || !loginRequest.IsValid()) { Response.StatusCode = 401; return Core.CreateErrorResponse(Result.InvalidRequest).AsJson(); }
            if ((loginRequest.Password ?? "").Length < 6) { Response.StatusCode = 401; return Core.CreateErrorResponse(Result.InvalidCredentials).AsJson(); }
            IResponse response = Core.UserManager.Login(loginRequest);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("auth/registerWithCredentials")]
        public async Task<string> RegisterWithCredentials()
        {
            IRegisterRequest? registerRequest = await Core.GetFromBody<IRegisterRequest>(Request.Body);
            if (registerRequest == null || !registerRequest.IsValid()) { Response.StatusCode = 401; return Core.CreateErrorResponse(Result.InvalidRequest).AsJson(); }
            if ((registerRequest.Password ?? "").Length < 6) { Response.StatusCode = 401; return Core.CreateErrorResponse(Result.InvalidCredentials).AsJson(); }
            IResponse response = Core.UserManager.Register(registerRequest);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("auth/refreshAccessToken")]
        public async Task<string> RefreshAccessToken()
        {
            IRefreshTokenRequest? request = await Core.GetFromBody<IRefreshTokenRequest>(Request.Body);
            if (request == null || !request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest).AsJson();
            IResponse response = Core.UserManager.RefreshTokens(request);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("auth/generateJWT")]
        public async Task<string> GenerateJWTToken()
        {
            ITokenRequest? request = await Core.GetFromBody<ITokenRequest>(Request.Body);
            if (request == null || !request.IsValid())
                return Core.CreateErrorResponse(Result.InvalidRequest).AsJson();
            IResponse response = Core.UserManager.GenerateJWT(request);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("auth/checkJWT")]
        public async Task<string> CheckJWTToken()
        {
            IJWTLoginRequest? request = await Core.GetFromBody<IJWTLoginRequest>(Request.Body);
            if (request == null || !request.IsValid())
                return Core.CreateErrorResponse(Result.InvalidRequest).AsJson();
            IResponse response = Core.UserManager.CheckJWT(request);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("auth/loginWithJWT")]
        public async Task<string> LoginWithJWT()
        {
            IJWTLoginRequest? request = await Core.GetFromBody<IJWTLoginRequest>(Request.Body);
            if (request == null || !request.IsValid())
                return Core.CreateErrorResponse(Result.InvalidRequest).AsJson();
            IResponse response = Core.UserManager.AuthenticateJWT(request);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("user/exists")] //TODO: GET FUNC with /{uid?}
        public async Task<string> UserExists()
        {
            return Core.CreateErrorResponse(Result.NotImplemented).AsJson();
        }

        [HttpPost]
        [Route("user/{uid?}")]
        public async Task<string> GetUser(string? uid = null)
        {
            IGetUserRequest? userRequest = await Core.GetFromBody<IGetUserRequest>(Request.Body);
            if (userRequest == null || !userRequest.IsValid()) { Response.StatusCode = 401; return Core.CreateErrorResponse(Result.InvalidRequest).AsJson(); }
            IResponse response = Core.UserManager.GetUser(userRequest);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }



        private int ResolveStatusCode(IResponse response, bool post = true)
        {
            if (response is IErrorResponse)
            {
                IErrorResponse res = (IErrorResponse)response;
                return res.Error.ToLower().Contains("internal") ? 500 : 401;
            }
            else
                return post ? 201 : 200;
        }
    }
}