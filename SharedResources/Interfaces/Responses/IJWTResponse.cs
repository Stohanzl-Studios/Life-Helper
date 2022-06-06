using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces.Responses
{
    public interface IJWTResponse : IResponse
    {
        [JsonProperty("token")]
        public string? Token { get; set; }
    }
}
