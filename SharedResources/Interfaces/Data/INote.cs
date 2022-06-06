using Newtonsoft.Json;
using SharedResources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces.Data
{
    public interface INote
    {
        [JsonProperty("title")]
        string Title { get; set; }
        [JsonProperty("content")]
        string? Content { get; set; }

        [JsonProperty("uid")]
        string? UID { get; }
        [JsonProperty("nid")]
        string? NID { get; }

        [JsonProperty("creationDate")]
        DateTime? CreationDate { get; set; }

        [JsonProperty("lastEdit")]
        DateTime LastEdit { get; set; }

        [JsonProperty("categories")]
        string[]? Categories { get; set; }

        [JsonProperty("visibility")]
        Visibility Visibility { get; set; }
    }
}
