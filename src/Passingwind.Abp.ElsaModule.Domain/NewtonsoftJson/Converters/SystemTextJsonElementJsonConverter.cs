using System;
using System.Text.Json;
using Newtonsoft.Json;

namespace Passingwind.Abp.ElsaModule.NewtonsoftJson.Converters;

public class SystemTextJsonElementJsonConverter : JsonConverter<JsonElement>
{
    public override bool CanRead => false;
    public override bool CanWrite => true;

    public override JsonElement ReadJson(JsonReader reader, Type objectType, JsonElement existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, JsonElement value, Newtonsoft.Json.JsonSerializer serializer)
    {
        writer.WriteRawValue(value.GetRawText());
    }
}
