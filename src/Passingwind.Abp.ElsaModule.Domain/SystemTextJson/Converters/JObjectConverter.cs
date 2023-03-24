using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Passingwind.Abp.ElsaModule.SystemTextJson.Converters;

public class JObjectConverter : JsonConverter<JObject>
{
    public override JObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonString = reader.GetString();

        if (!string.IsNullOrEmpty(jsonString))
            return JObject.Parse(jsonString);

        return default;
    }

    public override void Write(Utf8JsonWriter writer, JObject value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString(Newtonsoft.Json.Formatting.None)); // JsonNode.Parse(value.ToString()).AsObject()
    }
}
