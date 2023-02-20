using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public interface ICSharpTypeDefinitionService : ITransientDependency
{
    Task<CSharpTypeDefinition> GenerateAsync(WorkflowDefinition workflowDefinition, CancellationToken cancellationToken = default);
}
