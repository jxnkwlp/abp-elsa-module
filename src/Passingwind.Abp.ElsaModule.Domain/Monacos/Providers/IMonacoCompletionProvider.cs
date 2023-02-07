using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;

public interface IMonacoCompletionProvider : IScopedDependency
{
    Task<MonacoCompletionResult> HandleAsync(MonacoCompletionRequest request, CancellationToken cancellationToken = default);
}
