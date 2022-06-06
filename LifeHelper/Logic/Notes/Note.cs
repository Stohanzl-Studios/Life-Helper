using Newtonsoft.Json;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;

namespace LifeHelper.Logic.Notes
{
    public class Note : INote
    {
        public Note() { }
        public Note(string title, string? content, DateTime? creationDate, string[]? categories, Visibility visibility, string uid, string nid, DateTime lastEdit)
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
