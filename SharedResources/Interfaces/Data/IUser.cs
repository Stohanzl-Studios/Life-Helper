using Newtonsoft.Json;
using SharedResources.Enums;

namespace SharedResources.Interfaces.Data
{
    public interface IUser
    {
        string? Username { get; set; }
        string? Tag { get; set; }
        string? UID { get; set; }
        string? Email { get; set; }
        bool? EmailVerified { get; set; }
        bool? Disabled { get; set; }

        string[] Friends { get; set; }

        [JsonProperty("access_token", NullValueHandling = NullValueHandling.Ignore)]
        string? AccessToken { get; set; }
        [JsonProperty("refresh_token", NullValueHandling = NullValueHandling.Ignore)]
        string? RefreshToken { get; set; }
        [JsonProperty("expires_in", NullValueHandling = NullValueHandling.Ignore)]
        DateTime? ExpiresIn { get; set; }

        bool IsActive { get { return GetRemainingActiveTime().TotalSeconds <= 1 ? false : true; } }
        TimeSpan GetRemainingActiveTime() => (ExpiresIn ?? DateTime.Now).Subtract(DateTime.Now);
        void SetRemainingActiveTime(int seconds) { ExpiresIn = DateTime.Now.AddSeconds(seconds); }
        bool IsValid() => Username != null && Email != null && UID != null;
    }
}
