using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Passingwind.Abp.ElsaModule;

public static class Extensions
{
    public static TData SafeGetValue<TKey, TValue, TData>(this Dictionary<TKey, TValue> source, TKey key)
    {
        if (source.TryGetValue(key, out var value))
        {
            if (value is JToken token)
                return token.ToObject<TData>();

            else if (value is JsonElement jsonElement)
                return jsonElement.Deserialize<TData>();

            else
                return (TData)Convert.ChangeType(value, typeof(TData), CultureInfo.InvariantCulture);
        }
        return default;
    }
}
