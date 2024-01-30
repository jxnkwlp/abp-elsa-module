using System.Text;
using Elsa.Models;
using MediatR;
using Passingwind.CSharpScriptEngine;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

public class CSharpTypeDefinitionNotification : INotification
{
    public CSharpTypeDefinitionNotification(WorkflowDefinition workflowDefinition, StringBuilder definitionSource, CSharpScriptReference reference)
    {
        WorkflowDefinition = workflowDefinition;
        DefinitionSource = definitionSource;
        Reference = reference;
    }

    public WorkflowDefinition WorkflowDefinition { get; }
    public StringBuilder DefinitionSource { get; }
    public CSharpScriptReference Reference { get; set; }
}
