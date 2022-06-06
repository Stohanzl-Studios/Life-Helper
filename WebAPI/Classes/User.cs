using MySql.Data.MySqlClient;
using SharedResources.Enums;
using static SharedResources.Constants;
using SharedResources.Interfaces.Data;
using Newtonsoft.Json;

namespace WebAPI.Classes
{
    public class User : IUser
    {
        public User() { }
        public User(MySqlDataReader reader)
        {
            Username = reader.GetString("username");
            Tag = reader.GetString("tag");
            UID = reader.GetString("uid");
            Email = reader.GetString("email");
            EmailVerified = reader.GetBoolean("email_verified");
            Disabled = reader.GetBoolean("disabled");
            try { Friends = JsonConvert.DeserializeObject<dynamic>(reader.GetString("friends")).friends.ToObject<string[]>(); } catch { }
        }

        public string? Username { get; set; }
        public string? Tag { get; set; }
        public string? UID { get; set; }
        public string? Email { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? Disabled { get; set; }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        private DateTime? _ExpiresIn;
        public DateTime? ExpiresIn
        {
            get { return _ExpiresIn; }
            set
            {
                _ExpiresIn = value;
                WaitAndRemoveFromActiveUsers();
            }
        }
        private CancellationTokenSource? _ActiveCancel { get; set; }

        public string[] Friends { get; set; } = Array.Empty<string>();

        private void WaitAndRemoveFromActiveUsers()
        {
            if (_ActiveCancel != null) _ActiveCancel.Cancel();
            _ActiveCancel = new CancellationTokenSource();
            CancellationToken ct = _ActiveCancel.Token;
            Task.Run(async () =>
            {
                if (ct.IsCancellationRequested) return;
                int half = (int)(ExpiresIn ?? DateTime.Now.AddMinutes(-1)).Subtract(DateTime.Now).TotalMilliseconds / 2;
                await Task.Delay(half);
                if (ct.IsCancellationRequested) return;
                await Task.Delay(half);
                if (ct.IsCancellationRequested) return;
                Core.UserManager.RemoveActiveUser(UID);
            }, ct);
        }
    }
}
