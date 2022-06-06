using Newtonsoft.Json;

namespace SharedResources.Interfaces.Requests
{
    public interface IGetUserRequest : ITokenRequest
    {
        [JsonProperty("uid", NullValueHandling = NullValueHandling.Include)]
        string? UID { get; set; }
    }
}
