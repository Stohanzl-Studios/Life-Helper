using Newtonsoft.Json;
using SharedResources.Interfaces.Requests;

namespace WebAPI.Classes.Requests
{
    internal class RegisterRequest : IRegisterRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

        public bool IsValid() => Username != null && Password != null && Email != null;
    }
}