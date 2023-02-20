using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Services.GlobalVariables;

public class CSharpScriptConfigureGlobalVariableHander : INotificationHandler<CSharpScriptEvaluationNotification>
{
    private readonly GlobalVariableDomainService _globalVariableDomainService;

    public CSharpScriptConfigureGlobalVariableHander(GlobalVariableDomainService globalVariableDomainService)
    {
        _globalVariableDomainService = globalVariableDomainService;
    }

    public Task Handle(CSharpScriptEvaluationNotification notification, CancellationToken cancellationToken)
    {
        var global = notification.EvaluationGlobal;

        // GetGlobalVariable
        global.Context.GetGlobalVariable = (Func<string, string>)((string name) => _globalVariableDomainService.GetValueFromCacheAsync(name).Result);

        return Task.CompletedTask;
    }
}
