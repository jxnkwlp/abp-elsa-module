using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;
using Passingwind.CSharpScriptEngine;

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

    public async Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext context, Action<CSharpScriptEvaluationGlobal> globalConfigure = null, CancellationToken cancellationToken = default)
    {
        CSharpScriptEvaluationGlobal scriptEvaluationGlobal = new CSharpScriptEvaluationGlobal(_logger);

        globalConfigure?.Invoke(scriptEvaluationGlobal);

        // 
        await _mediator.Publish(new CSharpScriptEvaluationNotification(expression, context, scriptEvaluationGlobal), cancellationToken);

        // 
        CSharpScriptConfigureNotification scriptConfigure = new(expression);
        await _mediator.Publish(scriptConfigure, cancellationToken);

        string scriptId = $"{context.WorkflowExecutionContext.WorkflowBlueprint.VersionId}_{context.ActivityId}".Replace("-", null);

        _logger.LogDebug("Evaluate csharp code with id '{ScriptId}' ", scriptId);

        CSharpScriptContext scriptContext = new(_logger, expression, scriptEvaluationGlobal, scriptConfigure.Reference.Assemblies, scriptConfigure.Reference.Imports)
        {
            ScriptId = scriptId,
        };

        Stopwatch sw = Stopwatch.StartNew();

        var resultValue = await _cSharpScriptHost.RunAsync(scriptContext, cancellationToken: cancellationToken);

        sw.Stop();

        _logger.LogDebug("Evaluate csharp code with id '{ScriptId}' in {Elapsed} ", scriptId, sw.Elapsed);

        return resultValue == null ? null : Convert.ChangeType(resultValue, returnType);
    }
}
