using SharedResources.Interfaces.Responses;

namespace WebAPI.Classes.Responses
{
    public class JWTResponse : IJWTResponse
    {
        public string? Token { get; set; }

        public bool IsValid() => Token != null;
    }
}
