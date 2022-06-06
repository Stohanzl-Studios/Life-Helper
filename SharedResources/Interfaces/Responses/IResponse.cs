using Newtonsoft.Json;

namespace SharedResources.Interfaces.Responses
{
    public interface IResponse
    {
        bool IsValid();
        string AsJson() => JsonConvert.SerializeObject(this);
    }
}
