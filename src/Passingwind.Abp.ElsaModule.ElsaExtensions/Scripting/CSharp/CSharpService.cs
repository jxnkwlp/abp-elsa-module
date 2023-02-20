using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.CSharp;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpService : ICSharpService
{
    private readonly ILogger<CSharpService> _logger;
    private readonly IMediator _mediator;
    private readonly ICSharpScriptHost _cSharpScriptHost;

    public CSharpService(ILogger<CSharpService> logger, IMediator mediator, ICSharpScriptHost cSharpScriptHost)
    {
        _logger = logger;
        _mediator = mediator;
        _cSharpScriptHost = cSharpScriptHost;
    }

    public async Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext activityExecutionContext, Action<CSharpScriptEvaluationGlobal> configure = null, CancellationToken cancellationToken = default)
    {
        CSharpScriptEvaluationGlobal scriptEvaluationGlobal = new CSharpScriptEvaluationGlobal();

        configure?.Invoke(scriptEvaluationGlobal);

        // 
        await _mediator.Publish(new CSharpScriptEvaluationNotification(expression, activityExecutionContext, scriptEvaluationGlobal));

        // 
        var scriptConfigure = new CSharpScriptConfigureNotification(expression);
        await _mediator.Publish(scriptConfigure);

        var scriptId = $"{activityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.VersionId}:{activityExecutionContext.ActivityId}";

        _logger.LogDebug($"Evaluate csharp code with id '{scriptId}' ");

        CSharpScriptContext context = new CSharpScriptContext(expression, scriptEvaluationGlobal, scriptConfigure.Reference.Assemblies, scriptConfigure.Reference.Imports);

        var resultValue = await _cSharpScriptHost.RunWithIdAsync(scriptId, context, (_) => { }, cancellationToken);

        if (resultValue == null)
            return null;

        return Convert.ChangeType(resultValue, returnType);
    }
}
