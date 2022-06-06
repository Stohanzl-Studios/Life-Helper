using MySql.Data.MySqlClient;

namespace WebAPI.Managers
{
    public class DatabaseManager
    {
        private DatabaseManager() { Database.Open(); }
        public static DatabaseManager Instance { get; } = new DatabaseManager();

        public MySqlConnection Database { get; } = new MySqlConnection("server=localhost;database=DATABASE_NAME;uid=root;pwd=DATABASE_PASSWORD;");
    }
}
