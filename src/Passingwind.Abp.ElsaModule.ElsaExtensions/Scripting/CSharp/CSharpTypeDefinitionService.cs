using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using MediatR;
using Passingwind.Abp.ElsaModule.CSharp;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpTypeDefinitionService : ICSharpTypeDefinitionService
{
    private readonly IMediator _mediator;

    public CSharpTypeDefinitionService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<CSharpTypeDefinition> GenerateAsync(WorkflowDefinition workflowDefinition, CancellationToken cancellationToken = default)
    {
        var reference = new CSharpScriptReference();

        var imports = reference.Imports;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("public static class Context { ");

        await _mediator.Publish<CSharpTypeDefinitionNotification>(new CSharpTypeDefinitionNotification(workflowDefinition, sb, reference));

        sb.AppendLine("}");

        return new CSharpTypeDefinition
        {
            Text = sb.ToString(),
            Assemblies = reference.Assemblies,
            Imports = reference.Imports,
        };
    }
}
