using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpService : ICSharpService
{
    private readonly IMediator _mediator;
    private readonly IOptions<CSharpScriptOptions> _options;
    private readonly ICSharpEvaluator _cSharpEvaluator;

    public CSharpService(IMediator mediator, IOptions<CSharpScriptOptions> options, ICSharpEvaluator cSharpEvaluator)
    {
        _mediator = mediator;
        _options = options;
        _cSharpEvaluator = cSharpEvaluator;
    }

    public async Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext activityExecutionContext, Action<CSharpEvaluationGlobal> configure, CancellationToken cancellationToken = default)
    {
        CSharpEvaluationGlobal scriptGlobal = new CSharpEvaluationGlobal();
        CSharpEvaluationContext evaluationContext = new CSharpEvaluationContext(_options.Value, scriptGlobal);

        await _mediator.Publish(new CSharpExpressionEvaluationNotification(expression, evaluationContext, activityExecutionContext));

        configure?.Invoke(scriptGlobal);

        var resultValue = await _cSharpEvaluator.EvaluateAsync(expression, returnType, evaluationContext, (options) => { }, cancellationToken);

        if (resultValue == null)
            return null;

        return Convert.ChangeType(resultValue, returnType);
    }
}
