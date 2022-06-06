using SharedResources.Interfaces.Requests;

namespace WebAPI.Classes.Requests
{
    public class GetUserRequest : IGetUserRequest
    {
        public string? UID { get; set; }
        public string? AccessToken { get; set; }

        public bool IsValid() => AccessToken != null;
    }
}
