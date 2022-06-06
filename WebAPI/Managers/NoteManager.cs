using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SharedResources;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;

namespace WebAPI.Managers
{
    public class NoteManager : INoteManager
    {
        private NoteManager() { }
        public static NoteManager Instance { get; } = new NoteManager();

        public IResponse GetPublicNotes(ITokenRequest? request, string[]? categories, int? offset)
        {
            IUser? user = null;
            if (request != null && request.IsValid())
                user = Core.UserManager.GetActiveUser(request.AccessToken);
            if (request != null && user == null)
                return Core.CreateErrorResponse(Result.InvalidAccessToken);
            List<INote> notes = new List<INote>();
            string categoryCommand = "";
            if (categories != null && categories.Length > 0)
            {
                foreach (string category in categories)
                    categoryCommand += $" OR JSON_EXTRACT(note.content, '$.categories') LIKE \"%\"{category}\"%\"";
                categoryCommand = categoryCommand.Remove(0, 3);
                categoryCommand = categoryCommand.Insert(0, " AND");
            }

            string commandString = "";
            if (user != null && user.Friends.Length > 0)
            {
                commandString = "SELECT * FROM note WHERE note.visibility = 'Public' AND (";
                for (int i = 0; i < user.Friends.Length; i++)
                {
                    if (i == 0)
                        commandString += $"note.uid = '{user.Friends[i]}'";
                    else
                        commandString += $"OR note.uid = '{user.Friends[i]}'";
                }
                commandString += (offset != null ? $"){categoryCommand} ORDER BY creation_date DESC LIMIT {30 + offset};" : $"){categoryCommand} ORDER BY creation_date DESC LIMIT 30;");
                using (MySqlCommand command = new MySqlCommand(commandString, Core.DatabaseManager.Database))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            notes.Add(Core.CreateINote(reader));
                    }
                }
            }
            commandString = "SELECT * FROM note WHERE note.visibility = \"Public\"";
            if (user != null && user.Friends.Length > 0)
                foreach (string friend in user.Friends)
                {
                    commandString += $" AND note.uid != '{friend}'";
                }
            commandString += $"{categoryCommand} ORDER BY creation_date DESC " + (offset != null ? $"LIMIT {30 + offset};" : "LIMIT 30;");
            using (MySqlCommand command = new MySqlCommand(commandString, Core.DatabaseManager.Database))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    for (int i = offset ?? -1; i >= 0; i--) if (!reader.NextResult()) break;
                    while (reader.Read())
                    {
                        INote note = Core.CreateINote(reader);
                        if (notes.Count == 0)
                        {
                            notes.Add(note);
                            continue;
                        }
                        int found = notes.FindIndex(x => x.CreationDate < note.CreationDate);
                        if (found == -1)
                            notes.Add(note);
                        else
                            notes.Insert(found, note);
                    }
                }
            }
            if (notes.Count > 0)
                notes.RemoveRange(notes.Count > 30 ? 29 : notes.Count - 1, notes.Count - 30 < 0 ? 0 : notes.Count - 30);
            return Core.CreateIGetNotesResponse(notes);
        }

        public IResponse GetUserNotes(ITokenRequest request, string[]? categories)
        {
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest);
            IUser? user = Core.UserManager.GetActiveUser(request.AccessToken ?? "");
            if (user == null) return Core.CreateErrorResponse(Result.InvalidAccessToken);
            List<INote> notes = new List<INote>();
            string categoryCommand = "";
            if (categories != null && categories.Length > 0)
            {
                foreach (string category in categories)
                    categoryCommand += $" OR JSON_EXTRACT(note.content, '$.categories') LIKE \"%\"{category}\"%\"";
                categoryCommand = categoryCommand.Remove(0, 3);
                categoryCommand = categoryCommand.Insert(0, " AND");
            }
            using (MySqlCommand command = new MySqlCommand($"SELECT * FROM note WHERE note.uid = @uid{categoryCommand} ORDER BY creation_date DESC;", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("uid", user.UID));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        notes.Add(Core.CreateINote(reader));
                }
            }
            return Core.CreateIGetNotesResponse(notes);
        }
        public IResponse SaveUserNote(IUserNoteRequest request)
        {
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest);
            IUser? user = Core.UserManager.GetActiveUser(request.AccessToken);
            if (user == null) return Core.CreateErrorResponse(Result.InvalidAccessToken);
            bool update = !String.IsNullOrEmpty(request.Note.NID);
            INote note = Core.CreateINote(request.Note.Title, request.Note.Content, (request.Note.NID != null ? (request.Note.CreationDate ?? DateTime.Now) : DateTime.Now), request.Note.Categories, request.Note.Visibility, user.UID, String.IsNullOrEmpty(request.Note.NID) ? GenerateId() : request.Note.NID, DateTime.Now);
            using (MySqlCommand command = new MySqlCommand(update ? "UPDATE note SET note.content = @content, note.visibility = @visibility, note.last_edit = @last_edit WHERE note.nid = @nid" : "INSERT INTO note(note.nid, note.uid, note.content, note.creation_date, note.visibility, note.last_edit) VALUES(@nid, @uid, @content, @creation_date, @visibility, @last_edit)", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("nid", note.NID));
                command.Parameters.Add(new MySqlParameter("uid", note.UID));
                command.Parameters.Add(new MySqlParameter("content", "{" + $"\"title\":\"{note.Title.Replace("\\", "\\\\").Replace("\n", "\\n")}\",\"content\":\"{(note.Content != null ? note.Content.Replace("\\", "\\\\").Replace("\n", "\\n") : "")}\"" + (note.Categories != null && note.Categories.Length > 0 ? $",\"categories\": {JsonConvert.SerializeObject(note.Categories)}" : "") + "}"));
                command.Parameters.Add(new MySqlParameter("creation_date", note.CreationDate));
                command.Parameters.Add(new MySqlParameter("visibility", EnumHelper.GetVisibilityName(note.Visibility)));
                command.Parameters.Add(new MySqlParameter("last_edit", note.LastEdit));
                try { command.ExecuteNonQuery(); } catch { return Core.CreateErrorResponse(Result.Internal); }
            }
            return Core.CreateIGetNotesResponse(new[] { note });
        }
        public IResponse DeleteUserNote(IUserNoteRequest request)
        {
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest);
            IUser? user = Core.UserManager.GetActiveUser(request.AccessToken);
            if (user == null) return Core.CreateErrorResponse(Result.InvalidAccessToken);
            if (user.UID != request.Note.UID) return Core.CreateErrorResponse(Result.Unauthorized);
            using (MySqlCommand command = new MySqlCommand("DELETE FROM note WHERE note.nid = @nid", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("nid", request.Note.NID ?? ""));
                try { command.ExecuteNonQuery(); } catch { return Core.CreateErrorResponse(Result.Internal); }
            }
            return Core.EmptyResponse;
        }


        private string GenerateId() => RandomGenerator.NextString(100);
    }
}
