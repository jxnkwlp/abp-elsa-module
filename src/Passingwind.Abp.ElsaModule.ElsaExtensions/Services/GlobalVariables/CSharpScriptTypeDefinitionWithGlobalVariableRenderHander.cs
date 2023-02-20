using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Services.GlobalVariables;

public class CSharpScriptTypeDefinitionWithGlobalVariableRenderHander : INotificationHandler<CSharpTypeDefinitionNotification>
{
    public Task Handle(CSharpTypeDefinitionNotification notification, CancellationToken cancellationToken)
    {
        var source = notification.DefinitionSource;

        // methods
        source.AppendLine(@" 
public static string GetGlobalVariable(string name)=> throw new System.NotImplementedException();  
");

        return Task.CompletedTask;
    }
}
