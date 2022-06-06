using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;

namespace LifeHelper.Logic.Notes.Requests
{
    public class UserNoteRequest : IUserNoteRequest
    {
        public UserNoteRequest(INote? note, string? accessToken)
        {
            Note = note;
            AccessToken = accessToken;
        }

        public INote? Note { get; set; }
        public string? AccessToken { get; set; }

        public bool IsValid() => AccessToken != null && Note != null;
    }
}
