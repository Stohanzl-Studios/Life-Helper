using SharedResources.Enums;
using SharedResources.Interfaces;
using SharedResources.Interfaces.Responses;
using LifeHelper.Logic.Enums;
using SharedResources.Interfaces.Data;

namespace LifeHelper.Delegates
{
    public delegate void OnUserStateChanged(UserState state);

    public delegate void OnAppStateChanged(AppState state);
    public class OnAppStateChangedInfo : IDelegateBase
    {
        public Result Result { get; private set; }
        public AppState AppState { get; private set; }

        public string? Error { get; private set; }
        public string? Message { get; private set; }
    }

    public delegate void OnHttpRequestCompleted(OnHttpRequestCompletedInfo info);
    public class OnHttpRequestCompletedInfo : IDelegateBase
    {
        public OnHttpRequestCompletedInfo(Result result, IResponse response, object? clientData)
        {
            Result = result;
            Data = clientData;
            Response = response;
            if (response is IErrorResponse)
            {
                IErrorResponse error = (IErrorResponse)response;
                Error = error.Error;
                Message = error.Message;
            }
        }

        public Result Result { get; private set; }
        public IResponse Response { get; private set; }
        public object? Data { get; private set; }

        public string? Error { get; private set; }
        public string? Message { get; private set; }
    }

    public delegate void OnGetNotesCompleted(OnGetNotesCompletedInfo data);
    public class OnGetNotesCompletedInfo : IDelegateBase
    {
        public OnGetNotesCompletedInfo(IDelegateBase data, IEnumerable<INote>? notes)
        {
            Result = data.Result;
            Error = data.Error;
            Message = data.Message;
            Notes = notes;
        }

        public Result Result { get; private set; }
        public string? Error { get; private set; }
        public string? Message { get; private set; }

        public IEnumerable<INote>? Notes { get; private set; }
    }

    public delegate void OnUserUpdated(IDelegateBase data);
    public delegate void OnSaveNoteCompleted(IDelegateBase data);
    public delegate void OnDeleteNoteCompleted(IDelegateBase data);
    public delegate void OnTokensRefreshed(IDelegateBase data);
    public delegate void OnUserRegistered(IDelegateBase data);
    public delegate void OnUserLoggedIn(IDelegateBase data);
    public delegate void OnJWTTokenChecked(bool valid);
    public class DelegateBaseInfo : IDelegateBase
    {
        public DelegateBaseInfo(Result result, string? error = null, string? message = null)
        {
            Result = result;
            Error = error;
            Message = message;
        }

        public Result Result { get; private set; }

        public string? Error { get; private set; }
        public string? Message { get; private set; }
    }
}
