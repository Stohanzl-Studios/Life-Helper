using LifeHelper.Delegates;

namespace LifeHelper.Logic.Managers
{
    public interface IHttpManager
    {
        Task PostObject(object data, string address, OnHttpRequestCompleted completedCallback, Type responseType, bool silent = false, object? clientData = null);
        Task PostString(string data, string address, OnHttpRequestCompleted completedCallback, Type responseType, bool silent = false, object? clientData = null);

        Task GetResponse(string address, OnHttpRequestCompleted completedCallback, Type responseType, bool silent = false, object? clientData = null);
    }
}
