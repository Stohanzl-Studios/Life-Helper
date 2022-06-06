using Newtonsoft.Json;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces.Requests
{
    public interface IUserNoteRequest : ITokenRequest
    {
        [JsonProperty("note")]
        INote? Note { get; set; }
    }
}
