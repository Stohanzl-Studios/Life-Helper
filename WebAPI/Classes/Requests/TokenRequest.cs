using SharedResources.Interfaces.Requests;

namespace WebAPI.Classes.Requests
{
    public class TokenRequest : ITokenRequest
    {
        public string? AccessToken { get; set; }

        public bool IsValid() => AccessToken != null;
    }
}
