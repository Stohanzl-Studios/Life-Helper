using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces.Responses
{
    public interface IErrorResponse : IResponse
    {
        [JsonProperty("message")]
        string? Message { get; set; }
        [JsonProperty("error")]
        string? Error { get; set; }
        [JsonProperty("code")]
        int? Code { get; set; }
    }
}
