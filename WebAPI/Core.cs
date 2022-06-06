using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;
using System.Text;
using WebAPI.Classes;
using WebAPI.Classes.Requests;
using WebAPI.Classes.Responses;
using WebAPI.Managers;

namespace WebAPI
{
    internal static class Core
    {
        public static DatabaseManager DatabaseManager => DatabaseManager.Instance;
        public static IUserManager UserManager => Managers.UserManager.Instance;
        public static INoteManager NoteManager => Managers.NoteManager.Instance;

        public static IResponse EmptyResponse => new BaseResponse();

        public static async Task<T?> GetFromBody<T>(Stream body) where T : IRequest
        {
            try
            {
                StreamReader bodyReader = new StreamReader(body);
                string bodyString = await bodyReader.ReadToEndAsync();
                bodyReader.Close();
                Type type = typeof(T);
#pragma warning disable CS8600
                if (typeof(T) == typeof(ILoginRequest)) return (T)(IRequest)JsonConvert.DeserializeObject<LoginRequest>(bodyString);
                else if (typeof(T) == typeof(IRegisterRequest)) return (T)(IRequest)JsonConvert.DeserializeObject<RegisterRequest>(bodyString);
                else if (typeof(T) == typeof(IGetUserRequest)) return (T)(IRequest)JsonConvert.DeserializeObject<GetUserRequest>(bodyString);
                else if (typeof(T) == typeof(IRefreshTokenRequest)) return (T)(IRequest)JsonConvert.DeserializeObject<RefreshTokenRequest>(bodyString);
                else if (typeof(T) == typeof(ITokenRequest)) return (T)(IRequest)JsonConvert.DeserializeObject<TokenRequest>(bodyString);
                else if (typeof(T) == typeof(IJWTLoginRequest)) return (T)(IRequest)JsonConvert.DeserializeObject<JWTLoginRequest>(bodyString);
                else if (typeof(T) == typeof(IUserNoteRequest)) return (T)(IRequest)JsonConvert.DeserializeObject<UserNoteRequest>(bodyString);
                else
                    throw new NotImplementedException("Type not implemented");
#pragma warning restore CS8600
            }
            catch { return default(T); }
        }

        public static IUser CreateIUser(MySqlDataReader? reader = null) =>
            reader == null ? new User() : new User(reader);
        public static INote CreateINote(MySqlDataReader? reader = null) =>
            reader == null ? new Note() : new Note(reader);
        public static INote CreateINote(string title, string? content, DateTime creationDate, string[]? categories, Visibility visibility, string uid, string nid, DateTime lastEdit) =>
            new Note(title, content, creationDate, categories, visibility, uid, nid, lastEdit);

        internal static byte[] JwtSecret = Encoding.UTF8.GetBytes("abd1cbe8b4e4222cf23a05b137d99d8c7b94411d2f4e2dc0a010e205");

        //Responses
        public static IErrorResponse CreateErrorResponse(string? error = null, string? message = null, int? code = null, int? statusCode = null) => new ErrorResponse(message, error, code);
        public static IErrorResponse CreateErrorResponse(Result error)
        {
            switch (error)
            {
                default:
                case Result.InvalidRequest: return new ErrorResponse("Invalid request body", "Check your request body", 0);
                case Result.InvalidCredentials: return new ErrorResponse("Invalid credentials", "Check your credentials", 1);
                case Result.AccountNotFound: return new ErrorResponse("Account doesn't exists", "Account not found", 2);
                case Result.Internal: return new ErrorResponse("Internal error", "Internal error happened. PLease try again later", 3);
                case Result.AccountAlreadyExists: return new ErrorResponse("Account already exists", "Account already exists", 4);
                case Result.UsernameAlreadyExists: return new ErrorResponse("Username already exists", "User with this username already exists", 5);
                case Result.NotImplemented: return new ErrorResponse("Not implemented", "Function is not implemented", -1);
                case Result.InvalidAccessToken: return new ErrorResponse("Access token invalid", "Please relogin", 6);
                case Result.Unauthorized: return new ErrorResponse("Unauthorized access", "You don't have access to see this profile", 7);
                case Result.AccountDisabled: return new ErrorResponse("Disabled Account", "Your account has been disabled", 8);
                case Result.InvalidRefreshToken: return new ErrorResponse("Refresh token invalid", "Refresh token expired or is invalid. Please relogin.", 9);
                case Result.InvalidJWTToken: return new ErrorResponse("Invalid JWT token", "Your JWT token is invalid. Please relogin", 10);
            }
        }
        public static IGetUserResponse CreateGetUserResponse(IUser user) => new GetUserResponse(user);
        public static ITokenResponse CreateITokenResponse() => new TokenResponse();
        public static ITokenResponse CreateITokenResponse(string access, string refresh, int expires) => new TokenResponse(access, refresh, expires);
        public static ITokenResponse CreateITokenResponse(IUser user) => new TokenResponse(user.AccessToken, user.RefreshToken, (int)user.GetRemainingActiveTime().TotalSeconds);
        public static IJWTResponse CreateIJWTResponse(string token) => new JWTResponse() { Token = token };

        public static IGetNotesResponse CreateIGetNotesResponse(IEnumerable<INote> notes) => new GetNotesResponse(notes.ToArray());
    }
}