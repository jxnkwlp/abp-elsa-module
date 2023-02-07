using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;

public interface IMonacoHoverInfoProvider : IScopedDependency
{
    Task<MonacoHoverInfoResult> HandleAsync(MonacoHoverInfoRequest request, CancellationToken cancellationToken = default);
}
