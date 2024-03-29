﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Permissions;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.Teams;

[Authorize(Policy = ElsaModulePermissions.WorkflowTeams.Default)]
public class WorkflowTeamAppService : ElsaModuleAppService, IWorkflowTeamAppService
{
    private readonly IWorkflowTeamManager _workflowTeamManager;
    private readonly IWorkflowTeamRepository _workflowTeamRepository;
    private readonly IPermissionManager _permissionManager;
    private readonly IdentityRoleManager _identityRoleManager;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;

    public WorkflowTeamAppService(
        IWorkflowTeamManager workflowTeamManager,
        IWorkflowTeamRepository workflowTeamRepository,
        IPermissionManager permissionManager,
        IdentityRoleManager identityRoleManager,
        IIdentityRoleRepository identityRoleRepository,
        IIdentityUserRepository identityUserRepository,
        IWorkflowDefinitionRepository workflowDefinitionRepository)
    {
        _workflowTeamManager = workflowTeamManager;
        _workflowTeamRepository = workflowTeamRepository;
        _permissionManager = permissionManager;
        _identityRoleManager = identityRoleManager;
        _roleRepository = identityRoleRepository;
        _userRepository = identityUserRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
    }

    public virtual async Task<PagedResultDto<WorkflowTeamBasicDto>> GetListAsync(WorkflowTeamListRequestDto input)
    {
        var count = await _workflowTeamRepository.GetCountAsync();
        var list = await _workflowTeamRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, nameof(WorkflowTeam.CreationTime) + " desc");

        return new PagedResultDto<WorkflowTeamBasicDto>()
        {
            Items = ObjectMapper.Map<List<WorkflowTeam>, List<WorkflowTeamBasicDto>>(list),
            TotalCount = count,
        };
    }

    public virtual async Task<WorkflowTeamDto> GetAsync(Guid id)
    {
        var entity = await _workflowTeamRepository.GetAsync(id);

        return ObjectMapper.Map<WorkflowTeam, WorkflowTeamDto>(entity);
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowTeams.Create)]
    public virtual async Task<WorkflowTeamDto> CreateAsync(WorkflowTeamCreateOrUpdateDto input)
    {
        if (await _workflowTeamRepository.AnyAsync(x => x.Name == input.Name.Trim()))
        {
            throw new UserFriendlyException($"The group name '{input.Name}' exists.");
        }

        var entity = new WorkflowTeam(GuidGenerator.Create(), input.Name.Trim(), input.Description);

        if (input.UserIds != null)
            entity.Users.AddRange(input.UserIds.ConvertAll(x => new WorkflowTeamUser(x)));

        await _workflowTeamRepository.InsertAsync(entity);

        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<WorkflowTeam, WorkflowTeamDto>(entity);
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowTeams.Update)]
    public virtual async Task<WorkflowTeamDto> UpdateAsync(Guid id, WorkflowTeamCreateOrUpdateDto input)
    {
        var entity = await _workflowTeamRepository.GetAsync(id);

        if (await _workflowTeamRepository.AnyAsync(x => x.Name == input.Name.Trim() && x.Id != entity.Id))
        {
            throw new UserFriendlyException($"The group name '{input.Name}' exists.");
        }

        entity.SetName(input.Name.Trim());
        entity.Description = input.Description;

        entity.Users.Clear();
        if (input.UserIds != null)
            entity.Users.AddRange(input.UserIds.ConvertAll(x => new WorkflowTeamUser(x)));

        await _workflowTeamRepository.UpdateAsync(entity);

        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<WorkflowTeam, WorkflowTeamDto>(entity);
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowTeams.Update)]
    public async Task SetUsersAsync(Guid id, WorkflowTeamUserUpdateRequestDto input)
    {
        var entity = await _workflowTeamRepository.GetAsync(id);

        entity.Users.Clear();
        if (input.UserIds != null)
            entity.Users.AddRange(input.UserIds.ConvertAll(x => new WorkflowTeamUser(x)));

        await _workflowTeamRepository.UpdateAsync(entity);

        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowTeams.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _workflowTeamRepository.DeleteAsync(id);
    }

    public async Task<ListResultDto<WorkflowTeamRoleScopeDto>> GetRoleScopesAsync(Guid id)
    {
        var entity = await _workflowTeamRepository.GetAsync(id);

        var roleScopes = entity.RoleScopes ?? new List<WorkflowTeamRoleScope>();

        return new ListResultDto<WorkflowTeamRoleScopeDto>(ObjectMapper.Map<List<WorkflowTeamRoleScope>, List<WorkflowTeamRoleScopeDto>>(roleScopes));
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowTeams.Update)]
    public async Task<ListResultDto<WorkflowTeamRoleScopeDto>> SetRoleScopeAsync(Guid id, WorkflowTeamRoleScopeCreateOrUpdateDto input)
    {
        var entity = await _workflowTeamRepository.GetAsync(id);

        var scope = entity.RoleScopes.Find(x => x.RoleName == input.RoleName);

        if (scope == null)
        {
            scope = new WorkflowTeamRoleScope(input.RoleName);

            entity.RoleScopes.Add(scope);
        }

        if (input.Items != null)
            scope.Values = input.Items.ConvertAll(x => new WorkflowTeamRoleScopeValue { ProviderName = x.ProviderName, ProviderValue = x.ProviderValue });

        await _workflowTeamRepository.UpdateAsync(entity);

        await CurrentUnitOfWork.SaveChangesAsync();

        return new ListResultDto<WorkflowTeamRoleScopeDto>(ObjectMapper.Map<List<WorkflowTeamRoleScope>, List<WorkflowTeamRoleScopeDto>>(entity.RoleScopes));
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowTeams.Update)]
    public async Task DeleteRoleScopeAsync(Guid id, string roleName)
    {
        var entity = await _workflowTeamRepository.GetAsync(id);

        entity.RoleScopes.RemoveAll(x => x.RoleName == roleName);

        await _workflowTeamRepository.UpdateAsync(entity);

        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
    {
        var list = await _roleRepository.GetListAsync(sorting: nameof(IdentityRole.Name));

        return new ListResultDto<IdentityRoleDto>(ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list));
    }

    public async Task<ListResultDto<IdentityUserDto>> GetAssignableUsersAsync(WorkflowTeamAssignableUserListRequestDto input)
    {
        var list = await _userRepository.GetListAsync(sorting: nameof(IdentityUser.UserName), maxResultCount: input.MaxResultCount, filter: input.Filter, roleId: input.RoleId);

        return new ListResultDto<IdentityUserDto>(ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(list));
    }

    public async Task<ListResultDto<WorkflowDefinitionBasicDto>> GetAssignableDefinitionAsync(WorkflowDefinitionListRequestDto input)
    {
        var list = await _workflowDefinitionRepository.GetPagedListAsync(
            0,
            input.MaxResultCount,
            name: input.Filter,
            isSingleton: input.IsSingleton,
            deleteCompletedInstances: input.DeleteCompletedInstances,
            channel: input.Channel,
            tag: input.Tag,
            groupId: input.GroupId,
            ordering: input.Sorting);

        return new ListResultDto<WorkflowDefinitionBasicDto>(ObjectMapper.Map<List<WorkflowDefinition>, List<WorkflowDefinitionBasicDto>>(list));
    }
}
