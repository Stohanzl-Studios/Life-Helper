using Newtonsoft.Json;
using SharedResources.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces.Responses
{
    public interface IGetNotesResponse : IResponse
    {
        [JsonProperty("notes")]
        IEnumerable<INote> Notes { get; set; }
    }
}
