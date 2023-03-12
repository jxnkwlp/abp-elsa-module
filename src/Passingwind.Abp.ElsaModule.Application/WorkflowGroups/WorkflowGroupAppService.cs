using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Passingwind.Abp.ElsaModule.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

[Authorize(Policy = ElsaModulePermissions.WorkflowGroup.Default)]
public class WorkflowGroupAppService : ElsaModuleAppService, IWorkflowGroupAppService
{
    private readonly IWorkflowGroupManager _workflowGroupManager;
    private readonly IWorkflowGroupRepository _workflowGroupRepository;
    private readonly IPermissionManager _permissionManager;
    private readonly IdentityRoleManager _identityRoleManager;

    public WorkflowGroupAppService(
        IWorkflowGroupManager workflowGroupManager,
        IWorkflowGroupRepository workflowGroupRepository,
        IPermissionManager permissionManager,
        IdentityRoleManager identityRoleManager)
    {
        _workflowGroupManager = workflowGroupManager;
        _workflowGroupRepository = workflowGroupRepository;
        _permissionManager = permissionManager;
        _identityRoleManager = identityRoleManager;
    }

    public virtual async Task<PagedResultDto<WorkflowGroupBasicDto>> GetListAsync(WorkflowGroupListRequestDto input)
    {
        var count = await _workflowGroupRepository.GetCountAsync();
        var list = await _workflowGroupRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, nameof(WorkflowGroup.CreationTime) + " desc");

        return new PagedResultDto<WorkflowGroupBasicDto>()
        {
            Items = ObjectMapper.Map<List<WorkflowGroup>, List<WorkflowGroupBasicDto>>(list),
            TotalCount = count,
        };
    }

    public virtual async Task<WorkflowGroupDto> GetAsync(Guid id)
    {
        var entity = await _workflowGroupRepository.GetAsync(id);

        var dto = ObjectMapper.Map<WorkflowGroup, WorkflowGroupDto>(entity);

        dto.WorkflowIds = await _workflowGroupManager.GetGrantedWorkflowIdsAsync(entity);

        return dto;
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowGroup.Create)]
    public virtual async Task<WorkflowGroupDto> CreateAsync(WorkflowGroupCreateOrUpdateDto input)
    {
        if (await _workflowGroupRepository.AnyAsync(x => x.Name == input.Name.Trim()))
        {
            throw new UserFriendlyException($"The group name '{input.Name}' exists.");
        }

        var role = await _identityRoleManager.GetByIdAsync(input.RoleId);

        var entity = new WorkflowGroup(GuidGenerator.Create(), input.Name.Trim(), input.Description, role.Id, role.Name)
        {
            Users = (input.UserIds?.Select(x => new WorkflowGroupUser(x))?.ToList())
        };

        await _workflowGroupRepository.InsertAsync(entity);

        await _workflowGroupManager.SetPermissionGrantsAsync(entity, input.WorkflowIds);

        var dto = ObjectMapper.Map<WorkflowGroup, WorkflowGroupDto>(entity);

        return dto;
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowGroup.Update)]
    public virtual async Task<WorkflowGroupDto> UpdateAsync(Guid id, WorkflowGroupCreateOrUpdateDto input)
    {
        var entity = await _workflowGroupRepository.GetAsync(id);

        if (await _workflowGroupRepository.AnyAsync(x => x.Name == input.Name.Trim() && x.Id != entity.Id))
        {
            throw new UserFriendlyException($"The group name '{input.Name}' exists.");
        }

        var role = await _identityRoleManager.GetByIdAsync(input.RoleId);

        entity.Name = input.Name.Trim();
        entity.Description = input.Description;
        entity.RoleId = input.RoleId;
        entity.RoleName = role.Name;
        entity.Users = (input.UserIds?.Select(x => new WorkflowGroupUser(x))?.ToList());

        await _workflowGroupRepository.UpdateAsync(entity);

        await _workflowGroupManager.SetPermissionGrantsAsync(entity, input.WorkflowIds);

        return ObjectMapper.Map<WorkflowGroup, WorkflowGroupDto>(entity);
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowGroup.Update)]
    public async Task SetUsersAsync(Guid id, WorkflowGroupUpdateUsersRequestDto input)
    {
        var entity = await _workflowGroupRepository.GetAsync(id);

        entity.Users = (input.UserIds?.Select(x => new WorkflowGroupUser(x))?.ToList());

        await _workflowGroupRepository.UpdateAsync(entity);
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowGroup.Update)]
    public async Task SetWorkflowflowsAsync(Guid id, WorkflowGroupSetWorkflowRequestDto input)
    {
        var entity = await _workflowGroupRepository.GetAsync(id);

        await _workflowGroupManager.SetPermissionGrantsAsync(entity, input.WorkflowIds);
    }

    [Authorize(Policy = ElsaModulePermissions.WorkflowGroup.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _workflowGroupRepository.DeleteAsync(id);
    } 
}
