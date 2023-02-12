using System;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson.JsonConverters;
using Volo.Abp.Timing;

namespace Passingwind.WorkflowApp.Web.Services;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(AbpDateTimeConverter))]
public class MyDateTimeConverter : AbpDateTimeConverter, ITransientDependency
{
    private readonly IClock _clock;
    private readonly AbpJsonOptions _options;

    public MyDateTimeConverter(IClock clock, IOptions<AbpJsonOptions> abpJsonOptions) : base(clock, abpJsonOptions)
    {
        _clock = clock;
        _options = abpJsonOptions.Value;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var d2))
            return _clock.Normalize(d2);

        if (DateTime.TryParse(reader.GetString(), out var d3))
            return _clock.Normalize(d3);

        throw new JsonException("Can't get datetime from the reader!");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        if (_options.DefaultDateTimeFormat.IsNullOrWhiteSpace())
            writer.WriteStringValue(_clock.Normalize(value));
        else
        {
            writer.WriteStringValue(_clock.Normalize(value).ToString(_options.DefaultDateTimeFormat, CultureInfo.CurrentUICulture));
        }
    }
}

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(AbpNullableDateTimeConverter))]
public class MyNullableDateTimeConverter : AbpNullableDateTimeConverter, ITransientDependency
{
    private readonly IClock _clock;
    private readonly AbpJsonOptions _options;

    public MyNullableDateTimeConverter(IClock clock, IOptions<AbpJsonOptions> abpJsonOptions) : base(clock, abpJsonOptions)
    {
        _clock = clock;
        _options = abpJsonOptions.Value;
    }

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var d2))
            return _clock.Normalize(d2);

        if (DateTime.TryParse(reader.GetString(), out var d3))
            return _clock.Normalize(d3);

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
        {
            if (_options.DefaultDateTimeFormat.IsNullOrWhiteSpace())
                writer.WriteStringValue(_clock.Normalize(value.Value));
            else
            {
                writer.WriteStringValue(_clock.Normalize(value.Value).ToString(_options.DefaultDateTimeFormat, CultureInfo.CurrentUICulture));
            }
        }
    }
}
