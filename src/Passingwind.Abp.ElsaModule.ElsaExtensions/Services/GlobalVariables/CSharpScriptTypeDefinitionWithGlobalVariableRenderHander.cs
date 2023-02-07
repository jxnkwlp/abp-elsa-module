using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Services.GlobalVariables;

public class CSharpScriptTypeDefinitionWithGlobalVariableRenderHander : INotificationHandler<CSharpScriptTypeDefinitionNotification>
{
    public Task Handle(CSharpScriptTypeDefinitionNotification notification, CancellationToken cancellationToken)
    {
        var source = notification.CSharpTypeDefinitionSource;

        // properties 
        // methods
        source.AppendLine(@" 
public static string GetGlobalVariable(string name)=> throw new System.NotImplementedException();  
");

        return Task.CompletedTask;
    }
}
