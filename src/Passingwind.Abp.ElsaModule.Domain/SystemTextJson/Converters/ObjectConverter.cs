using System;
using System.Text.Json;
using Newtonsoft.Json;

namespace Passingwind.Abp.ElsaModule.SystemTextJson.Converters;

public class ObjectConverter : System.Text.Json.Serialization.JsonConverter<object>
{
    private static readonly Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings(JsonConvert.DefaultSettings?.Invoke())
    {
        // when handle object, need this config
        TypeNameHandling = TypeNameHandling.Auto,
    };

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True: return true;
            case JsonTokenType.False: return false;
            case JsonTokenType.Number when reader.TryGetInt64(out long l): return l;
            case JsonTokenType.Number: return reader.GetDouble();
            case JsonTokenType.String when reader.TryGetDateTime(out DateTime datetime): return datetime;
            case JsonTokenType.String when reader.TryGetGuid(out var guid): return guid;
            case JsonTokenType.String: return reader.GetString();
            case JsonTokenType.None:
            case JsonTokenType.Null:
            case JsonTokenType.Comment:
                return null;
        }

        var jsonString = JsonDocument.ParseValue(ref reader).RootElement.Clone().GetRawText();
        return Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, JsonSerializerSettings);
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(JsonConvert.SerializeObject(value, JsonSerializerSettings));
    }
}
