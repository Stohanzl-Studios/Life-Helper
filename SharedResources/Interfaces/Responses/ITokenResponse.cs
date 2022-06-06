using Newtonsoft.Json;

namespace SharedResources.Interfaces.Responses
{
    public interface ITokenResponse : IResponse
    {
        [JsonProperty("access_token")]
        string? AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        string? RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        int? ExpiresIn { get; set; }
    }
}
