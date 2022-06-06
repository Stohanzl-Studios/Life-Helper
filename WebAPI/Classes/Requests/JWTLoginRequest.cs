using SharedResources.Interfaces.Requests;

namespace WebAPI.Classes.Requests
{
    public class JWTLoginRequest : IJWTLoginRequest
    {
        public string JWTToken { get; set; }

        public bool IsValid() => JWTToken != null;
    }
}
