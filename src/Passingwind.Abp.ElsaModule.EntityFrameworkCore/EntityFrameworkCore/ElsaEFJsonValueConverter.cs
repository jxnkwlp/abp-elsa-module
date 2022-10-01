using System;
using Elsa.Serialization.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace EcsShop.EntityFrameworkCore
{
    public class ElsaEFJsonValueConverter<TModel> : ValueConverter<TModel, string>
    {
        public static Func<JsonSerializerSettings> Create;

        static ElsaEFJsonValueConverter()
        {
            Create = () =>
            {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, false) },
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                };
                settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                settings.Converters.Add(new TypeJsonConverter());
                settings.Converters.Add(new FlagEnumConverter(new DefaultNamingStrategy()));
                settings.Converters.Add(new TypeJsonConverter());
                settings.Converters.Add(new VersionOptionsJsonConverter());
                settings.Converters.Add(new InlineFunctionJsonConverter());

                return settings;
            };
        }


        public ElsaEFJsonValueConverter() : base(s => Serialize(s), x => Deserialize<TModel>(x))
        {
        }

        private static string Serialize<T>(T value)
        {
            var settings = Create();
            return JsonConvert.SerializeObject(value, Formatting.None, settings);
        }

        private static T Deserialize<T>(string json)
        {
            var settings = Create();
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
