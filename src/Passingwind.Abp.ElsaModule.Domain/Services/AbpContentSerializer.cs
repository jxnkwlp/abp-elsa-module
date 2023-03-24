using System;
using Elsa.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Volo.Abp.Json;

namespace Passingwind.Abp.ElsaModule.Services;

public class AbpContentSerializer : IContentSerializer
{
    private readonly IJsonSerializer _jsonSerializer;

    public AbpContentSerializer(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public T Deserialize<T>(JToken token)
    {
        return token.ToObject<T>();
    }

    public object Deserialize(JToken token, Type targetType)
    {
        return token.ToObject(targetType);
    }

    public T Deserialize<T>(string json)
    {
        return _jsonSerializer.Deserialize<T>(json);
    }

    public object Deserialize(string json, Type targetType)
    {
        return _jsonSerializer.Deserialize(targetType, json);
    }

    public object GetSettings()
    {
        return JsonConvert.DefaultSettings;
    }

    public string Serialize<T>(T value)
    {
        return _jsonSerializer.Serialize(value);
    }
}