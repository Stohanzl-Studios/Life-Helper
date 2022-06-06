using SharedResources.Interfaces.Responses;

namespace LifeHelper.Logic.Auth.Responses
{
    public class TokenResponse : ITokenResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }

        public bool IsValid() => AccessToken != null && RefreshToken != null && ExpiresIn != null;
    }
}