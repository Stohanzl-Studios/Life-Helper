using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces.Requests
{
    public interface ITokenRequest : IRequest
    {
        [JsonProperty("access_token")]
        [JsonRequired]
        string? AccessToken { get; set; }
    }
}
