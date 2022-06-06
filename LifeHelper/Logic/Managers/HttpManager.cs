using LifeHelper.Delegates;
using LifeHelper.Logic.Auth.Responses;
using LifeHelper.Logic.Enums;
using LifeHelper.Logic.Notes;
using LifeHelper.Logic.Notes.Responses;
using Newtonsoft.Json;
using SharedResources.Enums;
using SharedResources.Interfaces.Responses;
using System.Net;

namespace LifeHelper.Logic.Managers
{
    public class HttpManager : IHttpManager
    {
        private HttpManager() { }
        private static HttpManager _Instance = new HttpManager();
        public static HttpManager Instance { get => _Instance; }

        public async Task GetResponse(string address, OnHttpRequestCompleted completedCallback, Type responseType, bool silent = false, object? clientData = null)
        {
            HttpClient client = new HttpClient();
            if (!silent)
                Core.StateManager.ApplicationState = AppState.Loading;
            HttpResponseMessage response = new HttpResponseMessage();
            try { response = await client.GetAsync(address); } catch { Core.StateManager.ApplicationState = AppState.Normal; }
            string body = await response.Content.ReadAsStringAsync();
            if (body.Contains("\"error\":"))
            {
                IErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(body) ?? new ErrorResponse();
                try { completedCallback.Invoke(new OnHttpRequestCompletedInfo(Result.Failure, errorResponse, clientData)); } catch { }
                Core.StateManager.ApplicationState = AppState.Normal;
            }
            IResponse? decodedResponse = (IResponse?)GetFromBody(body, responseType);
            try { completedCallback.Invoke(new OnHttpRequestCompletedInfo(Result.Success, decodedResponse, clientData)); } catch { }
            Core.StateManager.ApplicationState = AppState.Normal;
            client.Dispose();
        }
        public async Task PostObject(object data, string address, OnHttpRequestCompleted completedCallback, Type responseType, bool silent = false, object? clientData = null) =>
            await PostString(JsonConvert.SerializeObject(data), address, completedCallback, responseType, silent, clientData);
        public async Task PostString(string data, string address, OnHttpRequestCompleted completedCallback, Type responseType, bool silent = false, object? clientData = null)
        {
            HttpClient client = new HttpClient();
            if (!silent)
                Core.StateManager.ApplicationState = AppState.Loading;
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            try
            {
                httpResponse = await client.PostAsync(address, new StringContent(data));
            }
            catch { Core.StateManager.ApplicationState = AppState.Normal; }
            string body = await httpResponse.Content.ReadAsStringAsync();
            if (body.Contains("\"error\":"))
            {
                IErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(body) ?? new ErrorResponse();
                completedCallback.Invoke(new OnHttpRequestCompletedInfo(Result.Failure, errorResponse, clientData));
                if (!silent)
                    Core.StateManager.ApplicationState = AppState.Normal;
                return;
            }

            IResponse? response = null;
            try { response = (IResponse?)GetFromBody(body, responseType); } catch { Core.StateManager.ApplicationState = AppState.Normal; }
            try
            {
                if (response == null)
                {
                    Core.StateManager.ApplicationState = AppState.Normal;
                    throw new Exception();
                }
                completedCallback.Invoke(new OnHttpRequestCompletedInfo(Result.Success, response, clientData));
            }
            catch
            {
                try
                {
                    completedCallback.Invoke(new OnHttpRequestCompletedInfo(Result.Failure, new ErrorResponse() { Error = "Internal error" }, clientData));
                }
                catch { }
            }
            Core.StateManager.ApplicationState = AppState.Normal;
            client.Dispose();
        }

        private object? GetFromBody(string body, Type? convertType)
        {
            if (body.Contains("\"error\":"))
                return JsonConvert.DeserializeObject<ErrorResponse>(body);
            else if (convertType == typeof(ITokenResponse))
                return JsonConvert.DeserializeObject<TokenResponse>(body);
            else if (convertType == typeof(IGetUserResponse))
                return JsonConvert.DeserializeObject<GetUserResponse>(body);
            else if (convertType == typeof(IJWTResponse))
                return JsonConvert.DeserializeObject<JWTResponse>(body);
            else if (convertType == typeof(IGetNotesResponse))
                return JsonConvert.DeserializeObject<GetNotesResponse>(body);
            //ADD NEW RESPONSES HERE!!!
            /*else if(convertType == typeof(T))
                return JsonConvert.DeserializeObject<T>(body);*/
            else
                throw new NotImplementedException("Response type is not implemented!");
        }
    }
}