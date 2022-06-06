using SharedResources.Interfaces.Responses;

namespace WebAPI.Classes.Responses
{
    public class BaseResponse : IResponse
    {
        public bool IsValid() => true;
    }
}
