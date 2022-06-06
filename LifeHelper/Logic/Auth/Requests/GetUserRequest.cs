using SharedResources.Interfaces.Requests;

namespace LifeHelper.Logic.Auth.Requests
{
    public class GetUserRequest : IGetUserRequest
    {
        public GetUserRequest(string? uid, string? accessToken)
        {
            UID = uid;
            AccessToken = accessToken;
        }

        public string? UID { get; set; }
        public string? AccessToken { get; set; }

        public bool IsValid() => true;
    }
}
