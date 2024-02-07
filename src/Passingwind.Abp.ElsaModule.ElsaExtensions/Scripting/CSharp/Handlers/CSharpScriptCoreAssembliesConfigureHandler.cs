using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Handlers;

public class CSharpScriptCoreAssembliesConfigureHandler : INotificationHandler<CSharpScriptConfigureNotification>
{
    public Task Handle(CSharpScriptConfigureNotification notification, CancellationToken cancellationToken)
    {
        var reference = notification.Reference;

        reference.Assemblies.Add(Assembly.Load("Microsoft.Extensions.Logging.Abstractions"));
        reference.Assemblies.Add(Assembly.Load("Microsoft.Extensions.Logging"));

        reference.Imports.Add("Microsoft.Extensions.Logging");

        return Task.CompletedTask;
    }
}
