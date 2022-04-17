using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule;

public class AbpElsaTenantAccessor : ITenantAccessor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;

    public AbpElsaTenantAccessor(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public Task<string> GetTenantIdAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_currentTenant.Id?.ToString());
    }
}
