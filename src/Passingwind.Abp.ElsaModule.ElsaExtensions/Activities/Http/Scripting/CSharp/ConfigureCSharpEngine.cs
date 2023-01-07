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

public class ConfigureCSharpEngine : INotificationHandler<CSharpExpressionEvaluationNotification>
{
    private readonly IAbsoluteUrlProvider _absoluteUrlProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ConfigureCSharpEngine(IAbsoluteUrlProvider absoluteUrlProvider, IHttpContextAccessor httpContextAccessor)
    {
        _absoluteUrlProvider = absoluteUrlProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task Handle(CSharpExpressionEvaluationNotification notification, CancellationToken cancellationToken)
    {
        var activityExecutionContext = notification.ActivityExecutionContext;

        var evaluationContext = notification.Context;

        if (_httpContextAccessor.HttpContext == null)
            throw new System.Exception("The HttpContext is null");

        evaluationContext.EvaluationGlobal.Context.QueryString = (Func<string, string>)(key => _httpContextAccessor.HttpContext!.Request.Query[key].ToString());
        evaluationContext.EvaluationGlobal.Context.AbsoluteUrl = (Func<string, string>)(url => _absoluteUrlProvider.ToAbsoluteUrl(url).ToString());
        evaluationContext.EvaluationGlobal.Context.SignalUrl = (Func<string, string>)(signal => activityExecutionContext.GenerateSignalUrl(signal));
        evaluationContext.EvaluationGlobal.Context.GetRemoteIPAddress = (Func<string>)(() => _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress.ToString());
        evaluationContext.EvaluationGlobal.Context.GetRouteValue = (Func<string, object>)(key => _httpContextAccessor.HttpContext!.GetRouteValue(key));

        return Task.CompletedTask;
    }
}
