using Newtonsoft.Json;

namespace SharedResources.Interfaces.Requests
{
    public interface IUserExistsRequest : ITokenRequest
    {
        [JsonProperty("username")]
        string? Username { get; }
        [JsonProperty("uid")]
        string? UID { get; }
        [JsonProperty("email")]
        string? Email { get; }
    }
}