using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Handlers;

public class ConfigureCSharpEngine : INotificationHandler<CSharpEvaluationConfigureNotification>
{
    public Task Handle(CSharpEvaluationConfigureNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
