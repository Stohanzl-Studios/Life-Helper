using Newtonsoft.Json;
using SharedResources.Interfaces.Responses;
using System.Runtime.Serialization;

namespace WebAPI.Classes.Responses
{
    public class TokenResponse : ITokenResponse
    {
        public TokenResponse() { }
        public TokenResponse(string? access, string? refresh, int? expires)
        {
            AccessToken = access;
            RefreshToken = refresh;
            ExpiresIn = expires;
        }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }

        public bool IsValid() => AccessToken != null && RefreshToken != null && ExpiresIn != null;
    }
}
