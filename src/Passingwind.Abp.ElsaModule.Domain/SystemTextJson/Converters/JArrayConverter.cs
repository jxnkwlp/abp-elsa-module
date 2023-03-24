using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Passingwind.Abp.ElsaModule.SystemTextJson.Converters;

public class JArrayConverter : JsonConverter<JArray>
{
    public override JArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonString = reader.GetString();

        if (!string.IsNullOrEmpty(jsonString))
            return JArray.Parse(jsonString);

        return default;
    }

    public override void Write(Utf8JsonWriter writer, JArray value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString(Newtonsoft.Json.Formatting.None));
    }
}