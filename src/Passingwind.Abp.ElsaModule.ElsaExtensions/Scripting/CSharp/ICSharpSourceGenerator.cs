using System.Threading.Tasks;
using Elsa.Models;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public interface ICSharpSourceGenerator : ITransientDependency
{
    Task<CSharpSourceGeneratoResult> GenerateAsync(WorkflowDefinition workflowDefinition);
}
