using Newtonsoft.Json;
using SharedResources.Interfaces.Data;

namespace LifeHelper.Logic.Auth.JsonConverters
{
    public class IUserConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IUser);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            => serializer.Deserialize<User>(reader);

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            => serializer.Serialize(writer, value);
    }
}
