using Newtonsoft.Json;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Responses;
using WebAPI.Classes.JsonConverters;

namespace WebAPI.Classes.Responses
{
    public class GetNotesResponse : IGetNotesResponse
    {
        public GetNotesResponse(INote[] notes)
        {
            Notes = notes;
        }

        [JsonConverter(typeof(INoteConverter))]
        public IEnumerable<INote> Notes { get; set; }

        public bool IsValid() => Notes != null;
    }
}
