using SharedResources.Interfaces.Requests;

namespace LifeHelper.Logic.Auth.Requests
{
    public class RefreshTokenRequest : IRefreshTokenRequest
    {
        public RefreshTokenRequest(string? refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public string? RefreshToken { get; set; }

        public bool IsValid() => RefreshToken != null;
    }
}
