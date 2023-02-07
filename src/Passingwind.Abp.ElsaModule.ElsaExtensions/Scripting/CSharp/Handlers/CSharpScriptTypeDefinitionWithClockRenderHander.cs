using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NodaTime;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Handlers;
public class CSharpScriptTypeDefinitionWithClockRenderHander : INotificationHandler<CSharpScriptTypeDefinitionNotification>
{
    public Task Handle(CSharpScriptTypeDefinitionNotification notification, CancellationToken cancellationToken)
    {
        var reference = notification.TypeDefinitionReference;

        // assemblies
        reference.Assemblies.Add(typeof(Instant).Assembly);

        // imports
        reference.Imports.Add("NodaTime");

        return Task.CompletedTask;
    }
}
