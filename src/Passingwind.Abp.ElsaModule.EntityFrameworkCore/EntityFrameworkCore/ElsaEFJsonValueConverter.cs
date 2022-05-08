using Elsa.Serialization.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EcsShop.EntityFrameworkCore
{
    public class ElsaEFJsonValueConverter<TModel> : ValueConverter<TModel, string>
    {
        public static JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, false) },
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };

        static ElsaEFJsonValueConverter()
        {
            JsonSerializerSettings.Converters.Add(new TypeJsonConverter());
            JsonSerializerSettings.Converters.Add(new FlagEnumConverter(new DefaultNamingStrategy()));
            JsonSerializerSettings.Converters.Add(new TypeJsonConverter());
            JsonSerializerSettings.Converters.Add(new VersionOptionsJsonConverter());
            JsonSerializerSettings.Converters.Add(new InlineFunctionJsonConverter());

            //JsonSerializerSettings.Error += (e, s) =>
            //{
            //    // ignore error
            //    s.ErrorContext.Handled = true;
            //};
        }


        public ElsaEFJsonValueConverter() : base(s => Serialize(s), x => Deserialize<TModel>(x))
        {

        }

        private static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }
    }
}
