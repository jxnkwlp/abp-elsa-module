using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Security.Claims;

namespace Passingwind.Abp.ElsaModule.Permissions;

/// <summary>
///  Workflow team permission
/// </summary>
/// <remarks>
///
///
///  Permission key format:
///     Name => 'WorkflowTeam:{workflow-id}',
///     ProviderKey => {workflow-team-id},
///     ProviderName => WorkflowTeam
/// </remarks>
public class WorkflowTeamPermissionValueProvider : PermissionValueProvider
{
    public const string ProviderName = "WorkflowTeam";

    public override string Name => ProviderName;

    private readonly IWorkflowTeamManager _workflowTeamManager;

    public WorkflowTeamPermissionValueProvider(IPermissionStore permissionStore, IWorkflowTeamManager workflowTeamManager) : base(permissionStore)
    {
        _workflowTeamManager = workflowTeamManager;
    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        // check permission name whether or not 'WorkflowPermission'
        if (!context.Permission.Name.StartsWith(ElsaModulePermissions.GroupName))
        {
            return PermissionGrantResult.Undefined;
        }

        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;

        if (userId == null)
            return PermissionGrantResult.Undefined;

        var teams = await _workflowTeamManager.GetListByUserIdAsync(Guid.Parse(userId));

        // if user not has any in teams, ignore it.
        if (!teams.Any())
            return PermissionGrantResult.Undefined;

        // roles
        foreach (var roleName in teams.SelectMany(x => x.RoleScopes.Select(r => r.RoleName)).Distinct().ToArray())
        {
            if (await PermissionStore.IsGrantedAsync(context.Permission.Name, RolePermissionValueProvider.ProviderName, roleName))
            {
                return PermissionGrantResult.Granted;
            }
        }

        // groups
        foreach (var team in teams)
        {
            if (await PermissionStore.IsGrantedAsync(context.Permission.Name, Name, team.GetPermissionKey()))
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

        // check permission name whether or not 'WorkflowPermission'
        permissionNames = permissionNames.Where(x => x.StartsWith(ElsaModulePermissions.GroupName)).ToList();

        var result = new MultiplePermissionGrantResult(permissionNames.ToArray());

        if (permissionNames.IsNullOrEmpty())
        {
            return result;
        }

        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        if (userId == null)
        {
            return result;
        }

        var teams = await _workflowTeamManager.GetListByUserIdAsync(Guid.Parse(userId));

        // if user not has any in teams, ignore it.
        if (!teams.Any())
            return result;

        // roles
        foreach (var roleName in teams.SelectMany(x => x.RoleScopes.Select(r => r.RoleName)).Distinct().ToArray())
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

            if (permissionNames.IsNullOrEmpty())
            {
                break;
            }
        }

        return result;
    }
}
