using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Services.GlobalVariables;

public class ConfigureGlobalVariableExpressionEvaluation : INotificationHandler<CSharpExpressionEvaluationNotification>
{
    private readonly GlobalVariableDomainService _globalVariableDomainService;

    public ConfigureGlobalVariableExpressionEvaluation(GlobalVariableDomainService globalVariableDomainService)
    {
        _globalVariableDomainService = globalVariableDomainService;
    }

    public Task Handle(CSharpExpressionEvaluationNotification notification, CancellationToken cancellationToken)
    {
        var context = notification.Context;

        // GetGlobalVariable
        context.EvaluationGlobal.Context.GetGlobalVariable = (Func<string, string>)((string name) => _globalVariableDomainService.GetValueFromCacheAsync(name).Result);

        return Task.CompletedTask;
    }
}
