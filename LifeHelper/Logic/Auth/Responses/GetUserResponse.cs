using LifeHelper.Logic.Auth.JsonConverters;
using Newtonsoft.Json;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Responses;

namespace LifeHelper.Logic.Auth.Responses
{
    public class GetUserResponse : IGetUserResponse
    {
        [JsonConverter(typeof(IUserConverter))]
        public IUser? User { get; set; }

        public bool IsValid() => User != null;
    }
}
