using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Handlers;

public class CSharpScriptAbpAssembliesConfigureHandler : INotificationHandler<CSharpScriptConfigureNotification>
{
    public Task Handle(CSharpScriptConfigureNotification notification, CancellationToken cancellationToken)
    {
        var reference = notification.Reference;

        foreach (var item in Directory.GetFiles(AppContext.BaseDirectory, "Volo.*.dll"))
        {
            reference.Assemblies.Add(Assembly.LoadFile(item));
        }

        return Task.CompletedTask;
    }
}
