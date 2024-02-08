using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Permissions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamManager : DomainService, IWorkflowTeamManager
{
    private readonly IWorkflowTeamRepository _workflowTeamRepository;
    private readonly IPermissionGroupDefinitionRepository _permissionGroupDefinitionRepository;
    private readonly IPermissionDefinitionRepository _permissionDefinitionRepository;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IPermissionManager _permissionManager;
    private readonly IPermissionGrantRepository _permissionGrantRepository;

    public WorkflowTeamManager(
        IWorkflowTeamRepository workflowTeamRepository,
        IPermissionGroupDefinitionRepository permissionGroupDefinitionRepository,
        IPermissionDefinitionRepository permissionDefinitionRepository,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IPermissionManager permissionManager,
        IPermissionGrantRepository permissionGrantRepository)
    {
        _workflowTeamRepository = workflowTeamRepository;
        _permissionGroupDefinitionRepository = permissionGroupDefinitionRepository;
        _permissionDefinitionRepository = permissionDefinitionRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _permissionManager = permissionManager;
        _permissionGrantRepository = permissionGrantRepository;
    }

    [UnitOfWork]
    public virtual async Task EnsurePermissionDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        var group = await _permissionGroupDefinitionRepository.FindAsync(x => x.Name == ElsaModulePermissions.GroupName, cancellationToken: cancellationToken);
        if (group == null)
        {
            group = await _permissionGroupDefinitionRepository.InsertAsync(new PermissionGroupDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                $"L:ElsaModule,Permission:${ElsaModulePermissions.GroupName}"), cancellationToken: cancellationToken);
        }

        var parent = await _permissionDefinitionRepository.FindAsync(x => x.GroupName == ElsaModulePermissions.GroupName && x.Name == ElsaModulePermissions.Workflows.Default, cancellationToken: cancellationToken);

        if (parent == null)
        {
            parent = await _permissionDefinitionRepository.InsertAsync(new PermissionDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                ElsaModulePermissions.Workflows.Default,
                null,
                "Workflows"), cancellationToken: cancellationToken);
        }
    }

    [UnitOfWork]
    public virtual async Task InitialPermissionDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        var exists = await _permissionDefinitionRepository.GetListAsync(x => x.GroupName == ElsaModulePermissions.GroupName && x.ParentName == ElsaModulePermissions.Workflows.Default, cancellationToken: cancellationToken);

        var workflows = await _workflowDefinitionRepository.GetListAsync(cancellationToken: cancellationToken);

        foreach (var workflow in workflows)
        {
            var name = WorkflowHelper.GenerateWorkflowPermissionKey(workflow);

            if (exists.Any(x => x.Name == name))
            {
                continue;
            }

            await _permissionDefinitionRepository.InsertAsync(new PermissionDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                name,
                ElsaModulePermissions.Workflows.Default,
                $"Workflows:{workflow.Id:d}",
                multiTenancySide: workflow.TenantId == null ? Volo.Abp.MultiTenancy.MultiTenancySides.Host : Volo.Abp.MultiTenancy.MultiTenancySides.Tenant), true, cancellationToken);
        }
    }

    public async Task<IReadOnlyList<Guid>> GetGrantedWorkflowIdsAsync(WorkflowTeam workflowTeam, CancellationToken cancellationToken = default)
    {
        var granted = await _permissionGrantRepository.GetListAsync(WorkflowTeamPermissionValueProvider.ProviderName, workflowTeam.GetPermissionKey(), cancellationToken);

        var ids = granted.ConvertAll(x => Guid.Parse(x.Name.AsSpan(ElsaModulePermissions.Workflows.Default.Length + 1)));

        return ids.Distinct().ToList();
    }

    public async Task<IReadOnlyList<WorkflowTeam>> GetListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var groups = await _workflowTeamRepository.GetListAsync(x => x.Users.Any(u => u.UserId == userId), includeDetails: true, cancellationToken: cancellationToken);

        return groups.Distinct().ToList();
    }

    public async Task<IReadOnlyList<WorkflowTeam>> GetListByWorkflowIdAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        var name = WorkflowHelper.GenerateWorkflowPermissionKey(workflowId);
        // TODO
        var list = await _permissionGrantRepository.GetListAsync(cancellationToken: cancellationToken);
        var groupPermissionGrants = list.Where(x => x.Name == name && x.ProviderName == WorkflowTeamPermissionValueProvider.ProviderName);

        var groupIds = groupPermissionGrants.Select(x => x.ProviderKey).Select(x => Guid.Parse(x));

        if (!groupIds.Distinct().Any())
            return Array.Empty<WorkflowTeam>();

        return await _workflowTeamRepository.GetListAsync(x => groupIds.Contains(x.Id), cancellationToken: cancellationToken);
    }

    public async Task UpdatePermissionGrantsAsync(WorkflowTeam workflowTeam, CancellationToken cancellationToken = default)
    {
        // ensure data is fill
        await _workflowTeamRepository.EnsureCollectionLoadedAsync(workflowTeam, x => x.RoleScopes, cancellationToken: cancellationToken);
        await _workflowTeamRepository.EnsureCollectionLoadedAsync(workflowTeam, x => x.Users, cancellationToken: cancellationToken);

        var grantedWorkflowIds = await GetGrantedWorkflowIdsAsync(workflowTeam, cancellationToken);

        var teamWorkflowIds = workflowTeam
            .RoleScopes
            .SelectMany(x => x.Values.Where(v => v.ProviderName == WorkflowTeamRoleScopeValue.WorkflowProviderName).Select(v => v.ProviderValue))
            .Select(x => Guid.Parse(x))
            .ToArray();

        // delete
        foreach (var item in grantedWorkflowIds.Except(teamWorkflowIds))
        {
            await _permissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(item), WorkflowTeamPermissionValueProvider.ProviderName, workflowTeam.GetPermissionKey(), false);
        }

        // update
        foreach (var item in teamWorkflowIds.Except(grantedWorkflowIds))
        {
            await _permissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(item), WorkflowTeamPermissionValueProvider.ProviderName, workflowTeam.GetPermissionKey(), true);
        }
    }
}
