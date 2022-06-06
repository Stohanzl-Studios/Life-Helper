using Newtonsoft.Json;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;
using WebAPI.Classes.JsonConverters;

namespace WebAPI.Classes.Requests
{
    public class UserNoteRequest : IUserNoteRequest
    {
        public string? AccessToken { get; set; }

        [JsonConverter(typeof(INoteConverter))]
        public INote? Note { get; set; }

        public bool IsValid() => !string.IsNullOrWhiteSpace(AccessToken) && Note != null;
    }
}
