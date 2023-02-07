using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Default;
public class MonacoCodeAnalysisProvider : IMonacoCodeAnalysisProvider
{
    public Task<MonacoCodeAnalysisResult> HandleAsync(MonacoCodeAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new MonacoCodeAnalysisResult());
    }
}
