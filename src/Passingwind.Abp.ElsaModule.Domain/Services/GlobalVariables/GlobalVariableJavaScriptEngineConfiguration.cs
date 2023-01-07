using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.JavaScript.Events;
using Elsa.Scripting.JavaScript.Messages;
using MediatR;
using Passingwind.Abp.ElsaModule.Common;

namespace Passingwind.Abp.ElsaModule.Services.GlobalVariables;

public class GlobalVariableJavaScriptExpression : INotificationHandler<EvaluatingJavaScriptExpression>
{
    private readonly GlobalVariableDomainService _globalVariableDomainService;

    public GlobalVariableJavaScriptExpression(GlobalVariableDomainService globalVariableDomainService)
    {
        _globalVariableDomainService = globalVariableDomainService;
    }

    public Task Handle(EvaluatingJavaScriptExpression notification, CancellationToken cancellationToken)
    {
        var engine = notification.Engine;

        engine.SetValue("getGlobalVariable", (Func<string, string>)((key) => _globalVariableDomainService.GetValueFromCacheAsync(key).Result));

        return Task.CompletedTask;
    }
}

public class GlobalVariableJavaScriptTypeDefinitions : INotificationHandler<RenderingTypeScriptDefinitions>
{
    public Task Handle(RenderingTypeScriptDefinitions notification, CancellationToken cancellationToken)
    {
        var output = notification.Output;

        output.AppendLine("declare function getGlobalVariable(name: string): string;");

        return Task.CompletedTask;
    }
}
