using System;
using System.Text.Json;
using Elsa.Scripting.JavaScript.Services;

namespace Passingwind.Abp.ElsaModule.Services;

public class SystemTextJsonJintConverter : IConvertsJintEvaluationResult
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConvertsJintEvaluationResult _convertsJintEvaluationResult;

    public SystemTextJsonJintConverter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var factory = new JintEvaluationResultConverterFactory(_serviceProvider);
        _convertsJintEvaluationResult = factory.GetConverter();
    }

    public object ConvertToDesiredType(object evaluationResult, Type desiredType)
    {
        if (evaluationResult == null)
            return null;

        var underlyingType = Nullable.GetUnderlyingType(desiredType) ?? desiredType;

        return evaluationResult switch
        {
            JsonElement jsonElement => jsonElement.Deserialize(underlyingType),

            _ => _convertsJintEvaluationResult.ConvertToDesiredType(evaluationResult, desiredType)
        };
    }
}
