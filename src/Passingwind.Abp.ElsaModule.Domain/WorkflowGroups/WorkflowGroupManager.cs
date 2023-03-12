using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Permissions;
using Volo.Abp.Domain.Services;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

public class WorkflowGroupManager : DomainService, IWorkflowGroupManager
{
    private readonly IWorkflowGroupRepository _workflowGroupRepository;
    private readonly IPermissionGroupDefinitionRepository _permissionGroupDefinitionRepository;
    private readonly IPermissionDefinitionRepository _permissionDefinitionRepository;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IPermissionManager _permissionManager;
    private readonly IPermissionGrantRepository _permissionGrantRepository;

    public WorkflowGroupManager(
        IWorkflowGroupRepository workflowGroupRepository,
        IPermissionGroupDefinitionRepository permissionGroupDefinitionRepository,
        IPermissionDefinitionRepository permissionDefinitionRepository,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IPermissionManager permissionManager,
        IPermissionGrantRepository permissionGrantRepository)
    {
        _workflowGroupRepository = workflowGroupRepository;
        _permissionGroupDefinitionRepository = permissionGroupDefinitionRepository;
        _permissionDefinitionRepository = permissionDefinitionRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _permissionManager = permissionManager;
        _permissionGrantRepository = permissionGrantRepository;
    }

    [UnitOfWork]
    public virtual async Task EnsureWorkflowGroupPermissionDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        var group = await _permissionGroupDefinitionRepository.FindAsync(x => x.Name == ElsaModulePermissions.GroupName);
        if (group == null)
        {
            group = await _permissionGroupDefinitionRepository.InsertAsync(new PermissionGroupDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                $"L:ElsaModule,Permission:${ElsaModulePermissions.GroupName}"));
        }

        var parent = await _permissionDefinitionRepository.FindAsync(x => x.GroupName == ElsaModulePermissions.GroupName && x.Name == ElsaModulePermissions.Workflow.Default);

        if (parent == null)
        {
            parent = await _permissionDefinitionRepository.InsertAsync(new PermissionDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                ElsaModulePermissions.Workflow.Default,
                null,
                "Workflows"));
        }
    }

    [UnitOfWork]
    public virtual async Task InitialWorkflowPermissionDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        var exists = await _permissionDefinitionRepository.GetListAsync(x => x.GroupName == ElsaModulePermissions.GroupName && x.ParentName == ElsaModulePermissions.Workflow.Default, cancellationToken: cancellationToken);

        var workflows = await _workflowDefinitionRepository.GetListAsync();

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
                ElsaModulePermissions.Workflow.Default,
                $"Workflows:{workflow.Id.ToString("d")}",
                multiTenancySide: Volo.Abp.MultiTenancy.MultiTenancySides.Both,
                providers: WorkflowGroupPermissionValueProvider.ProviderName), true, cancellationToken);
        }
    }

    public async Task<IEnumerable<Guid>> GetGrantedWorkflowIdsAsync(WorkflowGroup workflowGroup, CancellationToken cancellationToken = default)
    {
        var granted = await _permissionGrantRepository.GetListAsync(WorkflowGroupPermissionValueProvider.ProviderName, workflowGroup.GetPermissionKey());

        var ids = granted.ConvertAll(x => Guid.Parse(x.Name.Substring(ElsaModulePermissions.Workflow.Default.Length + 1)));

        return ids;
    }

    public async Task<IEnumerable<WorkflowGroup>> GetListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var groups = await _workflowGroupRepository.GetListAsync(x => x.Users.Any(u => u.UserId == userId), false, cancellationToken: cancellationToken);

        return groups.Distinct().ToList();
    }

    public async Task<IEnumerable<WorkflowGroup>> GetListByWorkflowIdAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        var name = WorkflowHelper.GenerateWorkflowPermissionKey(workflowId);
        // TODO
        var list = await _permissionGrantRepository.GetListAsync();
        var groupPermissionGrants = list.Where(x => x.Name == name && x.ProviderName == WorkflowGroupPermissionValueProvider.ProviderName);

        var groupIds = groupPermissionGrants.Select(x => x.ProviderKey).Select(x => Guid.Parse(x));

        if (!groupIds.Distinct().Any())
            return Array.Empty<WorkflowGroup>();

        return await _workflowGroupRepository.GetListAsync(x => groupIds.Contains(x.Id));
    }

    public async Task SetPermissionGrantsAsync(WorkflowGroup workflowGroup, IEnumerable<Guid> workflowIds, CancellationToken cancellationToken = default)
    {
        var grantedWorkflowIds = await GetGrantedWorkflowIdsAsync(workflowGroup);

        // delete
        foreach (var item in grantedWorkflowIds.Except(workflowIds))
        {
            await _permissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(item), WorkflowGroupPermissionValueProvider.ProviderName, workflowGroup.GetPermissionKey(), false);
        }

        // update
        foreach (var item in workflowIds.Except(grantedWorkflowIds))
        {
            await _permissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(item), WorkflowGroupPermissionValueProvider.ProviderName, workflowGroup.GetPermissionKey(), true);
        }
    }

    public async Task SetPermissionGrantAsync(WorkflowGroup workflowGroup, Guid workflowId, bool isGrant, CancellationToken cancellationToken = default)
    {
        await _permissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(workflowId), WorkflowGroupPermissionValueProvider.ProviderName, workflowGroup.GetPermissionKey(), isGrant);
    }
}