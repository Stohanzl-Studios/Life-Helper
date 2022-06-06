using Newtonsoft.Json;

namespace SharedResources.Interfaces.Requests
{
    public interface IRegisterRequest : IRequest
    {
        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }
    }
}
