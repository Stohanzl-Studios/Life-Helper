using SharedResources.Interfaces.Responses;

namespace WebAPI.Classes.Responses
{
    public class ErrorResponse : IErrorResponse
    {
        public ErrorResponse(string? error, string? message, int? code)
        {
            Message = message;
            Error = error;
            Code = code;
        }

        public string? Message { get; set; }
        public string? Error { get; set; }
        public int? Code { get; set; }

        public bool IsValid() => Error != null && Message != null && Code != null;
    }
}
