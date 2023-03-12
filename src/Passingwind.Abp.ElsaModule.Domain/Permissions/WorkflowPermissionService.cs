using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowPermissionService : IWorkflowPermissionService
{
    protected IGuidGenerator GuidGenerator { get; }
    protected ICurrentPrincipalAccessor PrincipalAccessor { get; }
    protected IPermissionManager PermissionManager { get; }
    protected IPermissionStore PermissionStore { get; }
    protected IPermissionGrantRepository PermissionGrantRepository { get; }
    protected IPermissionGroupDefinitionRepository PermissionGroupDefinitionRepository { get; }
    protected IPermissionDefinitionRepository PermissionDefinitionRepository { get; }
    protected IWorkflowDefinitionRepository WorkflowDefinitionRepository { get; }
    protected IWorkflowGroupManager WorkflowGroupManager { get; }
    protected IWorkflowGroupRepository WorkflowGroupRepository { get; }
    protected IDistributedCache<PermissionGrantCacheItem> PermissionCache { get; }
    protected IIdentityUserRepository IdentityUserRepository { get; }

    public WorkflowPermissionService(
        IGuidGenerator guidGenerator,
        ICurrentPrincipalAccessor principalAccessor,
        IPermissionManager permissionManager,
        IPermissionStore permissionStore,
        IPermissionGrantRepository permissionGrantRepository,
        IPermissionGroupDefinitionRepository permissionGroupDefinitionRepository,
        IPermissionDefinitionRepository permissionDefinitionRepository,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowGroupManager workflowGroupManager,
        IDistributedCache<PermissionGrantCacheItem> permissionCache,
        IIdentityUserRepository identityUserRepository)
    {
        GuidGenerator = guidGenerator;
        PrincipalAccessor = principalAccessor;
        PermissionManager = permissionManager;
        PermissionStore = permissionStore;
        PermissionGrantRepository = permissionGrantRepository;
        PermissionGroupDefinitionRepository = permissionGroupDefinitionRepository;
        PermissionDefinitionRepository = permissionDefinitionRepository;
        WorkflowDefinitionRepository = workflowDefinitionRepository;
        WorkflowGroupManager = workflowGroupManager;
        PermissionCache = permissionCache;
        IdentityUserRepository = identityUserRepository;
    }

    public virtual async Task<WorkflowPermissionGrantResult> GetGrantsAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.FindUserId();

        if (!userId.HasValue)
            return WorkflowPermissionGrantResult.Empty;

        var result = new Dictionary<Guid, IEnumerable<WorkflowPermissionGrantProvider>>();

        // get group permissions
        var groups = await WorkflowGroupManager.GetListByUserIdAsync(userId.Value, cancellationToken);

        if (groups.Any())
        {
            foreach (var group in groups)
            {
                var workflowIds = await WorkflowGroupManager.GetGrantedWorkflowIdsAsync(group, cancellationToken);

                foreach (var workflowId in workflowIds)
                {
                    if (result.ContainsKey(workflowId))
                        result[workflowId] = result[workflowId].Concat(new[] { new WorkflowPermissionGrantProvider(RolePermissionValueProvider.ProviderName, group.RoleName) }).ToList();
                    else
                        result[workflowId] = new[] { new WorkflowPermissionGrantProvider(RolePermissionValueProvider.ProviderName, group.RoleName) };
                }
            }
        }
        else
        {
            return WorkflowPermissionGrantResult.All;
        }

        // get user owners permission
        var userGrantedList = await PermissionGrantRepository.GetListAsync(WorkflowUserOwnerPermissionValueProvider.ProviderName, userId.Value.ToString("d"));

        foreach (var permissionGrant in userGrantedList)
        {
            var workflowId = WorkflowHelper.ResolveWorkflowIdFromPermissionKey(permissionGrant.Name);

            if (result.ContainsKey(workflowId))
                result[workflowId] = result[workflowId].Concat(new[] { new WorkflowPermissionGrantProvider(WorkflowUserOwnerPermissionValueProvider.ProviderName, permissionGrant.ProviderKey) }).ToList();
            else
                result[workflowId] = new[] { new WorkflowPermissionGrantProvider(WorkflowUserOwnerPermissionValueProvider.ProviderName, permissionGrant.ProviderKey) };
        }

        return new WorkflowPermissionGrantResult(result);
    }

    public virtual async Task<WorkflowPermissionGrantResult> GetGrantsAsync(CancellationToken cancellationToken = default)
    {
        return await GetGrantsAsync(PrincipalAccessor.Principal);
    }

    public virtual async Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, Guid workflowId, string name, CancellationToken cancellationToken = default)
    {
        var granted = await GetGrantsAsync(claimsPrincipal, cancellationToken);

        if (granted.AllGranted)
            return true;

        if (granted.WorkflowRoleMaps.TryGetValue(workflowId, out var providers))
        {
            foreach (var provider in providers)
            {
                // owners
                if (provider.Name == WorkflowUserOwnerPermissionValueProvider.ProviderName)
                    return true;

                // back to role permission
                return await PermissionStore.IsGrantedAsync(name, provider.Name, provider.Value);
            }
        }

        return false;
    }

    public virtual async Task<bool> IsGrantedAsync(Guid workflowId, string name, CancellationToken cancellationToken = default)
    {
        return await IsGrantedAsync(PrincipalAccessor.Principal, workflowId, name);
    }

    public virtual async Task SetUserWorkflowPermissionGrantAsync(WorkflowDefinition workflowDefinition, IdentityUser user, bool isGranted, CancellationToken cancellationToken = default)
    {
        //await PermissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(workflowDefinition), UserPermissionValueProvider.ProviderName, user.Name, isGranted);
        //await PermissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(workflowDefinition), WorkflowUserPermissionValueProvider.ProviderName, user.Id.ToString("d"), isGranted);

        var entity = await PermissionGrantRepository.FindAsync(WorkflowHelper.GenerateWorkflowPermissionKey(workflowDefinition), WorkflowUserOwnerPermissionValueProvider.ProviderName, user.Id.ToString("d"));
        if (entity == null && isGranted)
        {
            entity = await PermissionGrantRepository.InsertAsync(new PermissionGrant(GuidGenerator.Create(), WorkflowHelper.GenerateWorkflowPermissionKey(workflowDefinition), WorkflowUserOwnerPermissionValueProvider.ProviderName, user.Id.ToString("d")));
        }
        if (entity != null && !isGranted)
        {
            await PermissionGrantRepository.DeleteAsync(entity);
        }

        await PermissionCache.RemoveAsync(
            PermissionGrantCacheItem.CalculateCacheKey(
                entity.Name,
                entity.ProviderName,
                entity.ProviderKey
            )
        );
    }

    [UnitOfWork]
    public virtual async Task EnsureWorkflowGroupPermissionDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        var group = await PermissionGroupDefinitionRepository.FindAsync(x => x.Name == ElsaModulePermissions.GroupName);
        if (group == null)
        {
            group = await PermissionGroupDefinitionRepository.InsertAsync(new PermissionGroupDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                $"L:ElsaModule,Permission:${ElsaModulePermissions.GroupName}"));
        }

        var parent = await PermissionDefinitionRepository.FindAsync(x => x.GroupName == ElsaModulePermissions.GroupName && x.Name == ElsaModulePermissions.Workflow.Default);

        if (parent == null)
        {
            parent = await PermissionDefinitionRepository.InsertAsync(new PermissionDefinitionRecord(
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
        var exists = await PermissionDefinitionRepository.GetListAsync(x => x.GroupName == ElsaModulePermissions.GroupName && x.ParentName == ElsaModulePermissions.Workflow.Default, cancellationToken: cancellationToken);

        var workflows = await WorkflowDefinitionRepository.GetListAsync();

        foreach (var workflow in workflows)
        {
            var name = WorkflowHelper.GenerateWorkflowPermissionKey(workflow);

            if (exists.Any(x => x.Name == name))
            {
                continue;
            }

            await PermissionDefinitionRepository.InsertAsync(new PermissionDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                name,
                ElsaModulePermissions.Workflow.Default,
                $"Workflows:{workflow.Id.ToString("d")}",
                multiTenancySide: Volo.Abp.MultiTenancy.MultiTenancySides.Both,
                providers: WorkflowGroupPermissionValueProvider.ProviderName), true, cancellationToken);
        }
    }

    public virtual async Task CreateWorkflowPermissionDefinitionsAsync(WorkflowDefinition workflowDefinition, CancellationToken cancellationToken = default)
    {
        await PermissionDefinitionRepository.InsertAsync(new PermissionDefinitionRecord(
                 GuidGenerator.Create(),
                 ElsaModulePermissions.GroupName,
                 WorkflowHelper.GenerateWorkflowPermissionKey(workflowDefinition),
                 ElsaModulePermissions.Workflow.Default,
                 $"Workflows:{workflowDefinition.Id.ToString("d")}",
                 multiTenancySide: Volo.Abp.MultiTenancy.MultiTenancySides.Both,
                 providers: WorkflowGroupPermissionValueProvider.ProviderName), true, cancellationToken);
    }

    public async Task<IEnumerable<IdentityUser>> GetWorkflowOwnersAsync(WorkflowDefinition workflowDefinition, CancellationToken cancellationToken = default)
    {
        var name = WorkflowHelper.GenerateWorkflowPermissionKey(workflowDefinition.Id);

        // TODO
        var list = await PermissionGrantRepository.GetListAsync();
        var grants = list.Where(x => x.Name == name && x.ProviderName == WorkflowUserOwnerPermissionValueProvider.ProviderName);

        var userIds = grants.Select(x => x.ProviderKey).Select(x => Guid.Parse(x));

        if (!userIds.Distinct().Any())
            return Array.Empty<IdentityUser>();

        // TODO
        var users = await IdentityUserRepository.GetListAsync(cancellationToken: cancellationToken);

        return users.Where(x => userIds.Contains(x.Id));
    }

    public async Task AddWorkflowOwnerAsync(WorkflowDefinition workflowDefinition, IdentityUser user, CancellationToken cancellationToken = default)
    {
        await SetUserWorkflowPermissionGrantAsync(workflowDefinition, user, true, cancellationToken);
    }

    public async Task RemoveWorkflowOwnerAsync(WorkflowDefinition workflowDefinition, IdentityUser user, CancellationToken cancellationToken = default)
    {
        await SetUserWorkflowPermissionGrantAsync(workflowDefinition, user, false, cancellationToken);
    }
}
