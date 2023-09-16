using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using WorkflowDefinition = Passingwind.Abp.ElsaModule.Common.WorkflowDefinition;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowPermissionProvider : IWorkflowPermissionProvider
{
    protected IGuidGenerator GuidGenerator { get; }
    protected ICurrentPrincipalAccessor PrincipalAccessor { get; }
    protected IPermissionManager PermissionManager { get; }
    protected IPermissionStore PermissionStore { get; }
    protected IPermissionGrantRepository PermissionGrantRepository { get; }
    protected IPermissionGroupDefinitionRepository PermissionGroupDefinitionRepository { get; }
    protected IPermissionDefinitionRepository PermissionDefinitionRepository { get; }
    protected IWorkflowDefinitionRepository WorkflowDefinitionRepository { get; }
    protected IWorkflowTeamManager WorkflowTeamManager { get; }
    protected IDistributedCache<PermissionGrantCacheItem> PermissionCache { get; }
    protected IIdentityUserRepository IdentityUserRepository { get; }
    protected IIdentityRoleRepository IdentityRoleRepository { get; }
    protected IDistributedCache DistributedCache { get; }
    protected AbpDistributedCacheOptions CacheOptions { get; }
    protected IWorkflowTeamRepository WorkflowTeamRepository { get; }


    public WorkflowPermissionProvider(
        IGuidGenerator guidGenerator,
        ICurrentPrincipalAccessor principalAccessor,
        IPermissionManager permissionManager,
        IPermissionStore permissionStore,
        IPermissionGrantRepository permissionGrantRepository,
        IPermissionGroupDefinitionRepository permissionGroupDefinitionRepository,
        IPermissionDefinitionRepository permissionDefinitionRepository,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowTeamManager workflowTeamManager,
        IDistributedCache<PermissionGrantCacheItem> permissionCache,
        IIdentityUserRepository identityUserRepository,
        IDistributedCache distributedCache,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IIdentityRoleRepository identityRoleRepository,
        IWorkflowTeamRepository workflowTeamRepository)
    {
        GuidGenerator = guidGenerator;
        PrincipalAccessor = principalAccessor;
        PermissionManager = permissionManager;
        PermissionStore = permissionStore;
        PermissionGrantRepository = permissionGrantRepository;
        PermissionGroupDefinitionRepository = permissionGroupDefinitionRepository;
        PermissionDefinitionRepository = permissionDefinitionRepository;
        WorkflowDefinitionRepository = workflowDefinitionRepository;
        WorkflowTeamManager = workflowTeamManager;
        PermissionCache = permissionCache;
        IdentityUserRepository = identityUserRepository;
        DistributedCache = distributedCache;
        CacheOptions = cacheOptions.Value;
        IdentityRoleRepository = identityRoleRepository;
        WorkflowTeamRepository = workflowTeamRepository;
    }

    public virtual async Task<WorkflowPermissionGrantResult> GetGrantsAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.FindUserId();

        if (!userId.HasValue)
            return WorkflowPermissionGrantResult.Empty;

        var result = WorkflowPermissionGrantResult.Create();

        // get teams by user-id
        var teams = await WorkflowTeamManager.GetListByUserIdAsync(userId.Value, cancellationToken);

        if (!teams.Any())
        {
            return WorkflowPermissionGrantResult.Empty;
        }

        foreach (var team in teams)
        {
            foreach (var scope in team.RoleScopes)
            {
                var workflowIds = scope.Values.Select(v => Guid.Parse(v.ProviderValue)).ToArray();

                foreach (var value in workflowIds)
                {
                    result.AddProvider(value, RolePermissionValueProvider.ProviderName, scope.RoleName);
                }
            }
        }

        // get user owners 
        var userGrantedList = await PermissionGrantRepository.GetListAsync(WorkflowUserOwnerPermissionValueProvider.ProviderName, userId.Value.ToString("d"));

        foreach (var permissionGrant in userGrantedList)
        {
            var workflowId = WorkflowHelper.ResolveWorkflowIdFromPermissionKey(permissionGrant.Name);

            result.AddProvider(workflowId, WorkflowUserOwnerPermissionValueProvider.ProviderName, permissionGrant.ProviderKey);
        }

        return result;
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

        if (granted.WorkflowGrantProviders.TryGetValue(workflowId, out var providers))
        {
            foreach (var provider in providers)
            {
                // owners
                if (provider.Name == WorkflowUserOwnerPermissionValueProvider.ProviderName)
                    return true;

                // back to role permission
                if (await PermissionStore.IsGrantedAsync(name, provider.Name, provider.Value))
                    return true;
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
        await PermissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(workflowDefinition), WorkflowUserOwnerPermissionValueProvider.ProviderName, user.Id.ToString("d"), isGranted);
    }

    [UnitOfWork]
    public virtual async Task InitialWorkflowPermissionDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        // group
        var group = await PermissionGroupDefinitionRepository.FindAsync(x => x.Name == ElsaModulePermissions.GroupName);
        if (group == null)
        {
            group = await PermissionGroupDefinitionRepository.InsertAsync(new PermissionGroupDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                $"L:ElsaModule,Permission:${ElsaModulePermissions.GroupName}"));
        }

        // parent
        var parent = await PermissionDefinitionRepository.FindAsync(x => x.GroupName == ElsaModulePermissions.GroupName && x.Name == ElsaModulePermissions.Workflows.Default);

        if (parent == null)
        {
            parent = await PermissionDefinitionRepository.InsertAsync(new PermissionDefinitionRecord(
                GuidGenerator.Create(),
                ElsaModulePermissions.GroupName,
                ElsaModulePermissions.Workflows.Default,
                null,
                "Workflows"));
        }

        var exists = await PermissionDefinitionRepository.GetListAsync(x => x.GroupName == ElsaModulePermissions.GroupName && x.ParentName == ElsaModulePermissions.Workflows.Default, cancellationToken: cancellationToken);

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
                ElsaModulePermissions.Workflows.Default,
                $"Workflows:{workflow.Id.ToString("d")}",
                multiTenancySide: Volo.Abp.MultiTenancy.MultiTenancySides.Both
                ), true, cancellationToken);
        }
    }

    public virtual async Task CreateWorkflowPermissionDefinitionsAsync(WorkflowDefinition workflowDefinition, CancellationToken cancellationToken = default)
    {
        await PermissionDefinitionRepository.InsertAsync(new PermissionDefinitionRecord(
                 GuidGenerator.Create(),
                 ElsaModulePermissions.GroupName,
                 WorkflowHelper.GenerateWorkflowPermissionKey(workflowDefinition),
                 ElsaModulePermissions.Workflows.Default,
                 $"Workflows:{workflowDefinition.Id.ToString("d")}",
                 multiTenancySide: Volo.Abp.MultiTenancy.MultiTenancySides.Both), true, cancellationToken);

        await ClearPermissionDefinitionCacheAsync();
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

    [UnitOfWork]
    public async Task RestoreWorkflowTeamPermissionGrantsAsync(CancellationToken cancellationToken = default)
    {
        var teams = await WorkflowTeamRepository.GetListAsync(true);

        foreach (var team in teams)
        {
            foreach (var scope in team.RoleScopes)
            {
                foreach (var value in scope.Values)
                {
                    if (value.ProviderName == WorkflowTeamRoleScopeValue.WorkflowProviderName)
                        await PermissionManager.SetAsync(WorkflowHelper.GenerateWorkflowPermissionKey(Guid.Parse(value.ProviderValue)), WorkflowTeamPermissionValueProvider.ProviderName, team.Id.ToString("d"), true);
                    else
                    {
                        // TODO
                    }
                }

            }
        }
    }

    protected virtual string GetCommonStampCacheKey()
    {
        return $"{CacheOptions.KeyPrefix}_AbpInMemoryPermissionCacheStamp";
    }

    protected async Task ClearPermissionDefinitionCacheAsync()
    {
        // TODO friendly ?
        var cacheKey = GetCommonStampCacheKey();
        await DistributedCache.RemoveAsync(cacheKey);
    }
}
