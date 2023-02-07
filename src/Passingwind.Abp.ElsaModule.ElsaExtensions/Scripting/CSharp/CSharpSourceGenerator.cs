using System.Text;
using System.Threading.Tasks;
using Elsa.Models;
using MediatR;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpSourceGenerator : ICSharpSourceGenerator
{
    private readonly IMediator _mediator;

    public CSharpSourceGenerator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<CSharpSourceGeneratoResult> GenerateAsync(WorkflowDefinition workflowDefinition)
    {
        var reference = new CSharpScriptTypeDefinitionReference();

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("public static class Context { ");

        await _mediator.Publish<CSharpScriptTypeDefinitionNotification>(new CSharpScriptTypeDefinitionNotification(workflowDefinition, sb, reference));

        sb.AppendLine("}");

        return new CSharpSourceGeneratoResult
        {
            Code = sb.ToString(),
            Assemblies = reference.Assemblies,
            Imports = reference.Imports,
        };
    }
}
