using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Responses;

namespace WebAPI.Classes.Responses
{
    public class GetUserResponse : IGetUserResponse
    {
        public GetUserResponse(IUser user) => User = user;

        public IUser? User { get; set; }
        public bool IsValid() => User != null;
    }
}