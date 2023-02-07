using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;
public interface IMonacoCodeAnalysisProvider : IScopedDependency
{
    Task<MonacoCodeAnalysisResult> HandleAsync(MonacoCodeAnalysisRequest request, CancellationToken cancellationToken = default);
}
