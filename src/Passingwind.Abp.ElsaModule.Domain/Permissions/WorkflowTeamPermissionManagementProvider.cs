using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowTeamPermissionManagementProvider : PermissionManagementProvider
{
    public override string Name => WorkflowTeamPermissionValueProvider.ProviderName;

    public WorkflowTeamPermissionManagementProvider(IPermissionGrantRepository permissionGrantRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant) : base(permissionGrantRepository, guidGenerator, currentTenant)
    {
    }
}
