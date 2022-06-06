using SharedResources.Interfaces.Requests;

namespace WebAPI.Classes.Requests
{
    public class LoginRequest : ILoginRequest
    {
        public string? ID { get; set; }
        public string? Password { get; set; }

        public bool IsValid() => ID != null && Password != null;
    }
}
