using System;
using Newtonsoft.Json;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore
{
    public class TypeJsonConverter : JsonConverter<Type>
    {
        public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var fullName = reader.ReadAsString();
            return Type.GetType(fullName);
        }

        public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
        {
            writer.WriteValue(value.FullName);
        }
    }
}
