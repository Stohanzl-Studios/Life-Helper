using Newtonsoft.Json;

namespace SharedResources.Interfaces.Requests
{
    public interface ILoginRequest : IRequest
    {
        [JsonProperty("id")]
        string? ID { get; set; }
        [JsonProperty("password")]
        string? Password { get; set; }
    }
}
