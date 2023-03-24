using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Rebus.Extensions;

namespace Passingwind.Abp.ElsaModule.SystemTextJson.Converters;

public class TypeJsonConverter : JsonConverter<Type>
{
    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeName = reader.GetString();

        return typeName is not null and not "" ? Type.GetType(typeName) : default;
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        //if (value.IsGenericTypeDefinition)
        //    writer.WriteStringValue(value.GetGenericTypeDefinition().FullName);
        //else
        //    writer.WriteStringValue(value.FullName);

        writer.WriteStringValue(value.GetSimpleAssemblyQualifiedName());
    }
}
