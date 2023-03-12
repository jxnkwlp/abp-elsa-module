using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Security.Claims;

namespace Passingwind.Abp.ElsaModule.Permissions;

/// <summary>
///  Workflow group permission
/// </summary>
public class WorkflowGroupPermissionValueProvider : PermissionValueProvider
{
    public const string ProviderName = "WG";

    public override string Name => ProviderName;

    private readonly IWorkflowGroupManager _workflowGroupManager;

    public WorkflowGroupPermissionValueProvider(IPermissionStore permissionStore, IWorkflowGroupManager workflowGroupManager) : base(permissionStore)
    {
        _workflowGroupManager = workflowGroupManager;
    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;

        if (userId == null)
            return PermissionGrantResult.Undefined;

        var groups = await _workflowGroupManager.GetListByUserIdAsync(Guid.Parse(userId));

        if (!groups.Any())
            return PermissionGrantResult.Undefined;

        // roles
        var roles = groups.Select(x => x.RoleName).ToArray();

        foreach (var roleName in roles.Distinct())
        {
            if (await PermissionStore.IsGrantedAsync(context.Permission.Name, RolePermissionValueProvider.ProviderName, roleName))
            {
                return PermissionGrantResult.Granted;
            }
        }

        // groups
        foreach (var group in groups)
        {
            if (await PermissionStore.IsGrantedAsync(context.Permission.Name, Name, group.GetPermissionKey()))
            {
                return PermissionGrantResult.Granted;
            }
        }

        return PermissionGrantResult.Undefined;
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var permissionNames = context.Permissions.Select(x => x.Name).Distinct().ToList();
        Check.NotNullOrEmpty(permissionNames, nameof(permissionNames));

        var result = new MultiplePermissionGrantResult(permissionNames.ToArray());

        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        if (userId == null)
        {
            return result;
        }

        var groups = await _workflowGroupManager.GetListByUserIdAsync(Guid.Parse(userId));

        if (!groups.Any())
            return result;

        // roles
        var roles = groups.Select(x => x.RoleName).ToArray();

        foreach (var roleName in roles.Distinct())
        {
            var multipleResult = await PermissionStore.IsGrantedAsync(permissionNames.ToArray(), RolePermissionValueProvider.ProviderName, roleName);

            foreach (var grantResult in multipleResult.Result.Where(grantResult =>
                result.Result.ContainsKey(grantResult.Key) &&
                result.Result[grantResult.Key] == PermissionGrantResult.Undefined &&
                grantResult.Value != PermissionGrantResult.Undefined))
            {
                result.Result[grantResult.Key] = grantResult.Value;
                permissionNames.RemoveAll(x => x == grantResult.Key);
            }

            if (result.AllGranted || result.AllProhibited)
            {
                break;
            }
        }

        // groups
        foreach (var group in groups)
        {
            var multipleResult = await PermissionStore.IsGrantedAsync(permissionNames.ToArray(), Name, group.GetPermissionKey());

            foreach (var grantResult in multipleResult.Result.Where(grantResult =>
                         result.Result.ContainsKey(grantResult.Key) &&
                         result.Result[grantResult.Key] == PermissionGrantResult.Undefined &&
                         grantResult.Value != PermissionGrantResult.Undefined))
            {
                result.Result[grantResult.Key] = grantResult.Value;
                permissionNames.RemoveAll(x => x == grantResult.Key);
            }

            if (result.AllGranted || result.AllProhibited)
            {
                break;
            }

            if (permissionNames.IsNullOrEmpty())
            {
                break;
            }
        }

        return result;
    }
}
