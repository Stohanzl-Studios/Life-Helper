using Newtonsoft.Json;
using SharedResources.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces.Responses
{
    public interface IGetUserResponse : IResponse
    {
        [JsonProperty("user")]
        IUser? User { get; set; }
    }
}
