using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoProviders;

public interface ITabCompletionProvider : IScopedDependency
{
    Task<TabCompletionResult> HandleAsync(TabCompletionRequest request);
}
