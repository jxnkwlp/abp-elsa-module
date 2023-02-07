using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Default;
public class MonacoCodeFormatterProvider : IMonacoCodeFormatterProvider
{
    public Task<MonacoCodeFormatterResult> HandleAsync(MonacoCodeFormatterRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new MonacoCodeFormatterResult { Code = request.Code });
    }
}
