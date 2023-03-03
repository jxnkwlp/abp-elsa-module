using Elsa.Serialization.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace Passingwind.Abp.ElsaModule.MongoDB;

public class MongoJsonSerializerSettings
{
    private static JsonSerializerSettings _settings;

    static MongoJsonSerializerSettings()
    {
        _settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, false) },
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };
        // ignore error
        _settings.Error += JsonErrorHandle;
        _settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        _settings.Converters.Add(new TypeJsonConverter());
        _settings.Converters.Add(new FlagEnumConverter(new DefaultNamingStrategy()));
        _settings.Converters.Add(new TypeJsonConverter());
        _settings.Converters.Add(new VersionOptionsJsonConverter());
        _settings.Converters.Add(new InlineFunctionJsonConverter());
    }

    public static JsonSerializerSettings Settings { get => _settings; }

    public static void Update(JsonSerializerSettings settings)
    {
        _settings = settings;
    }

    private static void JsonErrorHandle(object sender, ErrorEventArgs e)
    {
        // ignore error.
        e.ErrorContext.Handled = true;
    }
}
