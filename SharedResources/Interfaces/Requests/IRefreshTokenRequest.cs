using Newtonsoft.Json;

namespace SharedResources.Interfaces.Requests
{
    public interface IRefreshTokenRequest : IRequest
    {
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
