using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Http.Contracts;
using Elsa.Activities.Http.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Activities.Http.Scripting.CSharp;

public class ConfigureCSharpEngine : INotificationHandler<CSharpScriptEvaluationNotification>
{
    private readonly IAbsoluteUrlProvider _absoluteUrlProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ConfigureCSharpEngine(IAbsoluteUrlProvider absoluteUrlProvider, IHttpContextAccessor httpContextAccessor)
    {
        _absoluteUrlProvider = absoluteUrlProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task Handle(CSharpScriptEvaluationNotification notification, CancellationToken cancellationToken)
    {
        var activityExecutionContext = notification.ActivityExecutionContext;
        var global = notification.EvaluationGlobal;

        if (_httpContextAccessor.HttpContext == null)
            return Task.CompletedTask;

        global.Context.QueryString = (Func<string, string>)(key => _httpContextAccessor.HttpContext!.Request.Query[key].ToString());
        global.Context.AbsoluteUrl = (Func<string, string>)(url => _absoluteUrlProvider.ToAbsoluteUrl(url).ToString());
        global.Context.SignalUrl = (Func<string, string>)(signal => activityExecutionContext.GenerateSignalUrl(signal));
        global.Context.GetRemoteIPAddress = (Func<string>)(() => _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress.ToString());
        global.Context.GetRouteValue = (Func<string, object>)(key => _httpContextAccessor.HttpContext!.GetRouteValue(key));

        return Task.CompletedTask;
    }
}
