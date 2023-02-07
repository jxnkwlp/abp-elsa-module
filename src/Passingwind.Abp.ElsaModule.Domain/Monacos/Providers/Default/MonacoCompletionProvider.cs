using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Default;
public class MonacoCompletionProvider : IMonacoCompletionProvider
{
    public Task<MonacoCompletionResult> HandleAsync(MonacoCompletionRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new MonacoCompletionResult());
    }
}
