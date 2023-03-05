using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Serialization.SystemTextJson;
using Passingwind.Abp.ElsaModule.Json.Converters;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

public class JsonValueConverter<TModel> : ValueConverter<TModel, string>
{
    public static Func<JsonSerializerOptions> Create;

    static JsonValueConverter()
    {
        Create = () =>
        {
            var settings = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                WriteIndented = false,
            };

            settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            settings.Converters.Add(new TypeJsonConverter());
            settings.Converters.Add(new JObjectConverter());
            settings.Converters.Add(new JArrayConverter());

            //settings.Converters.Add(new FlagEnumConverter(new DefaultNamingStrategy())); 
            //settings.Converters.Add(new VersionOptionsJsonConverter());
            //settings.Converters.Add(new InlineFunctionJsonConverter());

            return settings;
        };
    }

    public JsonValueConverter() : base(s => Serialize(s), x => Deserialize<TModel>(x))
    {
    }

    private static string Serialize<T>(T value)
    {
        var options = Create();
        return JsonSerializer.Serialize(value, options: options);
    }

    private static T Deserialize<T>(string json)
    {
        var options = Create();
        return JsonSerializer.Deserialize<T>(json, options);
    }
}
