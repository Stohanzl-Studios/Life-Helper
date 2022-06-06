using LifeHelper.Delegates;
using LifeHelper.Logic.Auth;
using LifeHelper.Logic.Auth.Requests;
using LifeHelper.Logic.Auth.Responses;
using LifeHelper.Logic.Calculator;
using LifeHelper.Logic.Enums;
using LifeHelper.Logic.Interfaces;
using LifeHelper.Logic.Managers;
using LifeHelper.Logic.Notes;
using LifeHelper.Logic.Notes.Requests;
using Microsoft.AspNetCore.Components;
using SharedResources.Enums;
using SharedResources.Interfaces;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;

namespace LifeHelper.Logic
{
    public static class Core
    {
        public static IHttpManager HttpManager => Managers.HttpManager.Instance;
        public static IUserManager UserManager => Managers.UserManager.Instance;
        public static IStateManager StateManager => Managers.StateManager.Instance;
        public static INoteManager NoteManager => Managers.NoteManager.Instance;
        

        public static IProblem CreateIProblem(IMathValue[]? values = null, MathOperation operation = MathOperation.None, MathOperation special = MathOperation.None) => new Problem() { Values = values, Operation = operation, Special = special };
        public static IProblemValue CreateIProblemValue(MathOperation operation = MathOperation.None, double? value = null, MathOperation special = MathOperation.None) => new ProblemValue() { Operation = operation, Value = value ?? 0, Special = special };
        public static INote CreateNote(string title, string? content, string[]? categories, Visibility visibility) => new Note(title, content, null, categories, visibility, (UserManager.UserState != UserState.LoggedOut ? UserManager.CurrentUser.UID ?? "" : ""), "", DateTime.Now);

        public static IRequest CreateRequest<T>(params object?[] objects) where T : IRequest
        {
            if (typeof(T) == typeof(ILoginRequest)) return new LoginRequest((string?)objects[0], (string?)objects[1]);
            else if (typeof(T) == typeof(IRefreshTokenRequest)) return new RefreshTokenRequest((string?)objects[0]);
            else if (typeof(T) == typeof(IGetUserRequest)) return new GetUserRequest((string?)objects[0], (string?)objects[1]);
            else if (typeof(T) == typeof(IRegisterRequest)) return new RegisterRequest((string?)objects[0], (string?)objects[1], (string?)objects[2]);
            else if (typeof(T) == typeof(ITokenRequest)) return new TokenRequest((string?)objects[0]);
            else if (typeof(T) == typeof(IJWTLoginRequest)) return new JWTLoginRequest((string?)objects[0]);
            else if (typeof(T) == typeof(IUserNoteRequest)) return new UserNoteRequest((INote?)objects[0], (string?)objects[1]);
            //else if (typeof(T) == typeof()) return new ();
            else
                return default(T);
        }

        public static IUser CreateUser() => new User();
        public static IDelegateBase CreateIDelegateBase(Result result, string? error = null, string? message = null) => new DelegateBaseInfo(result, error, message);
    }
}
