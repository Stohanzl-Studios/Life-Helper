using SharedResources.Enums;
using SharedResources.Interfaces.Data;

namespace LifeHelper.Logic.Auth
{
    public class User : IUser
    {
        public string? Username { get; set; }
        public string? Tag { get; set; }
        public string? UID { get; set; }
        public string? Email { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? Disabled { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
        public string[] Friends { get; set; }
    }
}
