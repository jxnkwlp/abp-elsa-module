using Elsa.Serialization.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Passingwind.Abp.ElsaModule.NewtonsoftJson.Converters;
using TypeJsonConverter = Passingwind.Abp.ElsaModule.NewtonsoftJson.Converters.TypeJsonConverter;

namespace Passingwind.Abp.ElsaModule.MongoDB;

public static class MongoJsonSerializerSettings
{
    public static JsonSerializerSettings Settings { get; set; }

    static MongoJsonSerializerSettings()
    {
        Settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, true) },
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            Formatting = Formatting.None,
        };
        // ignore error
        Settings.Error += JsonErrorHandle;
        Settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        Settings.Converters.Add(new TypeJsonConverter());
        Settings.Converters.Add(new FlagEnumConverter(new DefaultNamingStrategy()));
        Settings.Converters.Add(new VersionOptionsJsonConverter());
        Settings.Converters.Add(new InlineFunctionJsonConverter());
        Settings.Converters.Add(new SystemTextJsonElementJsonConverter());
    }

    private static void JsonErrorHandle(object sender, ErrorEventArgs e)
    {
        // ignore error.
        e.ErrorContext.Handled = true;
    }
}
