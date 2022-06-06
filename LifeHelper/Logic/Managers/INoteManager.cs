using LifeHelper.Delegates;
using SharedResources.Interfaces.Data;

namespace LifeHelper.Logic.Managers
{
    public interface INoteManager
    {
        bool GetUserNotes(OnGetNotesCompleted callback);
        bool GetPublicNotes(OnGetNotesCompleted callback);

        bool SaveUserNote(INote note, OnSaveNoteCompleted callback);
        bool DeleteUserNote(INote note, OnDeleteNoteCompleted callback);
    }
}
