using LifeHelper.Delegates;
using LifeHelper.Logic.Enums;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;
using static SharedResources.Constants;

namespace LifeHelper.Logic.Managers
{
    public class NoteManager : INoteManager
    {
        private NoteManager() { }
        public static NoteManager Instance { get; } = new NoteManager();

        public bool GetPublicNotes(OnGetNotesCompleted callback)
        {
            OnHttpRequestCompleted requestCallback = data =>
            {
                if (data.Result == Result.Success)
                {
                    IGetNotesResponse response = (IGetNotesResponse)data.Response;
                    try
                    {
                        callback.Invoke(new OnGetNotesCompletedInfo(data, response.Notes));
                    }
                    catch { }
                }
                else
                    try { callback.Invoke(new OnGetNotesCompletedInfo(data, null)); } catch { }
            };
            if (Core.UserManager.UserState != UserState.LoggedOut)
                Core.HttpManager.PostObject(Core.CreateRequest<ITokenRequest>(Core.UserManager.CurrentUser.AccessToken), WebApiAddress + "/note/browse", requestCallback, typeof(IGetNotesResponse));
            else
                Core.HttpManager.GetResponse(WebApiAddress + "/note/browse", requestCallback, typeof(IGetNotesResponse));
            return true;
        }
        public bool GetUserNotes(OnGetNotesCompleted callback)
        {
            Core.HttpManager.PostObject(Core.CreateRequest<ITokenRequest>(Core.UserManager.CurrentUser.AccessToken), WebApiAddress + "/note/user", data =>
            {
                if (data.Result == Result.Success)
                {
                    IGetNotesResponse response = (IGetNotesResponse)data.Response;
                    try
                    {
                        callback.Invoke(new OnGetNotesCompletedInfo(data, response.Notes));
                    }
                    catch { }
                }
                else
                    try { callback.Invoke(new OnGetNotesCompletedInfo(data, null)); } catch { }
            }, typeof(IGetNotesResponse));
            return true;
        }

        public bool SaveUserNote(INote note, OnSaveNoteCompleted callback)
        {
            if (note.Title != null)
                Core.HttpManager.PostObject(Core.CreateRequest<IUserNoteRequest>(note, Core.UserManager.UserState != UserState.LoggedOut ? Core.UserManager.CurrentUser.AccessToken : ""), WebApiAddress + "/note/save", data =>
                {
                    try { callback.Invoke(data); } catch { }
                }, typeof(IGetNotesResponse));
            else
                return false;
            return true;
        }
        public bool DeleteUserNote(INote note, OnDeleteNoteCompleted callback)
        {
            if (note.NID != null)
                Core.HttpManager.PostObject(Core.CreateRequest<IUserNoteRequest>(note, Core.UserManager.UserState != UserState.LoggedOut ? Core.UserManager.CurrentUser.AccessToken : ""), WebApiAddress + "/note/delete", data =>
                {
                    try { callback.Invoke(data); } catch { }
                }, typeof(IGetNotesResponse));
            else
                return false;
            return true;
        }
    }
}
