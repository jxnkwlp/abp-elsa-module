using System.Text;
using Elsa.Models;
using MediatR;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;
public class CSharpScriptTypeDefinitionNotification : INotification
{
    public CSharpScriptTypeDefinitionNotification(WorkflowDefinition workflowDefinition, StringBuilder cSharpTypeDefinitionSource, CSharpScriptTypeDefinitionReference typeDefinitionReference)
    {
        WorkflowDefinition = workflowDefinition;
        CSharpTypeDefinitionSource = cSharpTypeDefinitionSource;
        TypeDefinitionReference = typeDefinitionReference;
    }

    public WorkflowDefinition WorkflowDefinition { get; }
    //public CSharpTypeDefinitions TypeDefinitions { get; }
    public StringBuilder CSharpTypeDefinitionSource { get; }
    public CSharpScriptTypeDefinitionReference TypeDefinitionReference { get; }
}
