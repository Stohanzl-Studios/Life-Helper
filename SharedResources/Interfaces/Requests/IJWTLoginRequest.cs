using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces.Requests
{
    public interface IJWTLoginRequest : IRequest
    {
        [JsonProperty("jwt_token")]
        [JsonRequired]
        string JWTToken { get; set; }
    }
}
