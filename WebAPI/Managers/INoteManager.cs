using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;

namespace WebAPI.Managers
{
    public interface INoteManager
    {
        public IResponse GetUserNotes(ITokenRequest request, string[]? categories);
        public IResponse SaveUserNote(IUserNoteRequest request);
        public IResponse DeleteUserNote(IUserNoteRequest request);
        public IResponse GetPublicNotes(ITokenRequest? request, string[]? categories, int? offset);
    }
}
