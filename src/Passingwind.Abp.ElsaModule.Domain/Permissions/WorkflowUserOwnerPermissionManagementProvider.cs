using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowUserOwnerPermissionManagementProvider : PermissionManagementProvider
{
    public override string Name => WorkflowUserOwnerPermissionValueProvider.ProviderName;

    public WorkflowUserOwnerPermissionManagementProvider(IPermissionGrantRepository permissionGrantRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant) : base(permissionGrantRepository, guidGenerator, currentTenant)
    {
    }
}