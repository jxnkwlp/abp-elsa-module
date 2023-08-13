using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Security.Claims;

namespace Passingwind.Abp.ElsaModule.Permissions;

/// <summary>
///  workflow user owner permission provider
/// </summary>
public class WorkflowUserOwnerPermissionValueProvider : PermissionValueProvider
{
    public const string ProviderName = "WorkflowUserOwner";

    public override string Name => ProviderName;

    public WorkflowUserOwnerPermissionValueProvider(IPermissionStore permissionStore) : base(permissionStore)
    {
    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;

        if (userId == null)
            return PermissionGrantResult.Undefined;

        if (await PermissionStore.IsGrantedAsync(context.Permission.Name, Name, userId))
        {
            return PermissionGrantResult.Granted;
        }

        return PermissionGrantResult.Undefined;
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var permissionNames = context.Permissions.Select(x => x.Name).Distinct().ToArray();
        Check.NotNullOrEmpty(permissionNames, nameof(permissionNames));

        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        if (userId == null)
        {
            return new MultiplePermissionGrantResult(permissionNames);
        }

        return await PermissionStore.IsGrantedAsync(permissionNames, Name, userId);
    }
}
