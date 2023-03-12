using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowGroupPermissionManagementProvider : PermissionManagementProvider
{
    public override string Name => WorkflowGroupPermissionValueProvider.ProviderName;

    public WorkflowGroupPermissionManagementProvider(IPermissionGrantRepository permissionGrantRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant) : base(permissionGrantRepository, guidGenerator, currentTenant)
    {
    }
}
