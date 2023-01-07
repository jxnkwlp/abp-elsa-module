using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NodaTime;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Handlers;

public class ConfigureCSharpEngineWithClock : INotificationHandler<CSharpEvaluationConfigureNotification>
{
    public Task Handle(CSharpEvaluationConfigureNotification notification, CancellationToken cancellationToken)
    {
        var context = notification.Context;
        var scriptOptions = notification.ScriptOptions;

        notification.UpdateScriptOptions(scriptOptions
                            .AddImports("NodaTime")
                            .AddReferences(typeof(Instant).Assembly));

        return Task.CompletedTask;
    }
}
