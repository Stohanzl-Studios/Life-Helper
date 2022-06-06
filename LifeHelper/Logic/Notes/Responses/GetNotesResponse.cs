using LifeHelper.Logic.Notes.JsonConverters;
using Newtonsoft.Json;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Responses;

namespace LifeHelper.Logic.Notes.Responses
{
    public class GetNotesResponse : IGetNotesResponse
    {
        public GetNotesResponse() { }
        public GetNotesResponse(INote[] notes)
        {
            Notes = notes;
        }

        [JsonConverter(typeof(INoteConverter))]
        public IEnumerable<INote> Notes { get; set; }

        public bool IsValid() => Notes != null;
    }
}
