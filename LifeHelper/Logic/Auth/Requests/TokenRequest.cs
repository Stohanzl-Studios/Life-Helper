using SharedResources.Interfaces.Requests;

namespace LifeHelper.Logic.Auth.Requests
{
    public class TokenRequest : ITokenRequest
    {
        public TokenRequest(string? accessToken)
        {
            AccessToken = accessToken;
        }

        public string? AccessToken { get; set; }

        public bool IsValid() => AccessToken != null;
    }
}
