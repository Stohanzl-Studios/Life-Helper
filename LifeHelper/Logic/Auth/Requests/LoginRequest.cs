using SharedResources.Interfaces.Requests;

namespace LifeHelper.Logic.Auth.Requests
{
    public class LoginRequest : ILoginRequest
    {
        public LoginRequest() { }
        public LoginRequest(string? id, string? password)
        {
            ID = id;
            Password = password;
        }

        public string? ID { get; set; }
        public string? Password { get; set; }

        public bool IsValid() => ID != null && Password != null && Password.Length > 5;
    }
}
