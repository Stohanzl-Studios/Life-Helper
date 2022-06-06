using SharedResources.Interfaces.Requests;

namespace LifeHelper.Logic.Auth.Requests
{
    public class JWTLoginRequest : IJWTLoginRequest
    {
        public JWTLoginRequest(string? jWTToken)
        {
            JWTToken = jWTToken ?? "";
        }

        public string JWTToken { get; set; }

        public bool IsValid() => JWTToken != null;
    }
}
