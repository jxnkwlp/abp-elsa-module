using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;
using Passingwind.CSharpScriptEngine;

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

        var sb = new StringBuilder();

        sb = sb.AppendLine("public static class Context { ");

        await _mediator.Publish(new CSharpTypeDefinitionNotification(workflowDefinition, sb, reference), cancellationToken);

        sb = sb.AppendLine("}")
            .AppendLine($"public static {typeof(ILogger)} Logger {{get; set;}}");

        return new CSharpTypeDefinition
        {
            Text = sb.ToString(),
            Assemblies = reference.Assemblies,
            Imports = reference.Imports,
        };
    }
}
