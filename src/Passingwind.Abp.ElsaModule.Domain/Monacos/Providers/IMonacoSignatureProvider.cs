using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;
public interface IMonacoSignatureProvider : IScopedDependency
{
    Task<MonacoSignatureResult> HandleAsync(MonacoSignatureRequest request, CancellationToken cancellationToken = default);
}
