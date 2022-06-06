using SharedResources.Interfaces.Requests;

namespace WebAPI.Classes.Requests
{
    public class RefreshTokenRequest : IRefreshTokenRequest
    {
        public string? RefreshToken { get; set; }

        public bool IsValid() => RefreshToken != null;
    }
}
