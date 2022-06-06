using SharedResources.Interfaces.Requests;

namespace LifeHelper.Logic.Auth.Requests
{
    public class RegisterRequest : IRegisterRequest
    {
        public RegisterRequest(string? username, string? email, string? password)
        {
            Username = username;
            Email = email;
            Password = password;
        }

        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public bool IsValid() => Username != null && Password != null && Password.Length > 5 && Email != null;
    }
}
