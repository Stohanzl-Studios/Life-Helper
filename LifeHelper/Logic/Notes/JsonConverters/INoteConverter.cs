﻿using Newtonsoft.Json;
using SharedResources.Interfaces.Data;

namespace LifeHelper.Logic.Notes.JsonConverters
{
    public class INoteConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            => serializer.Deserialize<Note[]>(reader);

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            => serializer.Serialize(writer, value);
    }
}
