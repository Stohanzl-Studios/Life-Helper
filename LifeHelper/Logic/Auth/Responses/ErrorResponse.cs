using SharedResources.Interfaces.Responses;

namespace LifeHelper.Logic.Auth.Responses
{
    public class ErrorResponse : IErrorResponse
    {
        public string? Message { get; set; }
        public string? Error { get; set; }
        public int? Code { get; set; }

        public bool IsValid() => Message != null && Error != null && Code != null;
    }
}