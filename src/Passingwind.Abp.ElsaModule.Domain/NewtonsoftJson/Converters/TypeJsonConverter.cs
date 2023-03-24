using System;
using Newtonsoft.Json;
using Rebus.Extensions;

namespace Passingwind.Abp.ElsaModule.NewtonsoftJson.Converters;

public class TypeJsonConverter : JsonConverter<Type>
{ 
    public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
    {
        var typeName = value.GetSimpleAssemblyQualifiedName();
        serializer.Serialize(writer, typeName);
    }

    public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var typeName = serializer.Deserialize<string>(reader)!;
        return typeName is not null and not "" ? Type.GetType(typeName)! : default!;
    }
}
