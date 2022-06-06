using SharedResources.Interfaces.Responses;

namespace LifeHelper.Logic.Auth.Responses
{
    public class JWTResponse : IJWTResponse
    {
        public string? Token { get; set; }

        public bool IsValid() => Token != null;
    }
}
