using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;

namespace WebAPI.Classes
{
    public class Note : INote
    {
        public Note() { }
        public Note(MySqlDataReader reader)
        {
            dynamic noteContent = JsonConvert.DeserializeObject<dynamic>(reader.GetString("content")) ?? "";
            Title = ((string)noteContent.title).Replace("\\\\", "\\");
            try { Content = ((string)noteContent.content).Replace("\\\\", "\\"); } catch { }
            try { Categories = noteContent.categories.ToObject<string[]>(); } catch { }

            CreationDate = reader.GetDateTime("creation_date");

            Visibility = EnumHelper.GetVisibility(reader.GetString("visibility"));

            UID = reader.GetString("uid");
            NID = reader.GetString("nid");
        }
        public Note(string title, string? content, DateTime creationDate, string[]? categories, Visibility visibility, string uid, string nid, DateTime lastEdit)
        {
            Title = title;
            Content = content;
            CreationDate = creationDate;
            Categories = categories;
            Visibility = visibility;
            UID = uid;
            NID = nid;
            LastEdit = lastEdit;
        }

        public string Title { get; set; }
        public string? Content { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime LastEdit { get; set; }

        public string[]? Categories { get; set; }

        public Visibility Visibility { get; set; }

        public string UID { get; set; }
        public string NID { get; set; }
    }
}
