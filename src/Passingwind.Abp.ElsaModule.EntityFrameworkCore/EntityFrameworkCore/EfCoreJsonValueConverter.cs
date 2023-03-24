using System;
using Elsa.Serialization.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Serialization.SystemTextJson;
using Passingwind.Abp.ElsaModule.NewtonsoftJson.Converters;
using TypeJsonConverter = Elsa.Serialization.Converters.TypeJsonConverter;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

public class EfCoreJsonValueConverter<TModel> : ValueConverter<TModel, string>
{
    public static Func<JsonSerializerSettings> Create;

    static EfCoreJsonValueConverter()
    {
        Create = () =>
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, true) },
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.None,
            };
            // ignore error
            settings.Error += JsonErrorHandle;
            settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            settings.Converters.Add(new TypeJsonConverter());
            settings.Converters.Add(new FlagEnumConverter(new DefaultNamingStrategy()));
            settings.Converters.Add(new VersionOptionsJsonConverter());
            settings.Converters.Add(new InlineFunctionJsonConverter());
            settings.Converters.Add(new SystemTextJsonElementJsonConverter());

            return settings;
        };
    }

    public EfCoreJsonValueConverter() : base(s => Serialize(s), x => Deserialize<TModel>(x))
    {
    }

    private static void JsonErrorHandle(object sender, ErrorEventArgs e)
    {
        // ignore error.
        e.ErrorContext.Handled = true;
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