using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace Passingwind.Abp.ElsaModule.Teams;

[Area(ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[RemoteService(Name = ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[Route("api/elsa/workflow/teams")]
public class WorkflowTeamController : Controller, IWorkflowTeamAppService
{
    private readonly IWorkflowTeamAppService _service;

    public WorkflowTeamController(IWorkflowTeamAppService service)
    {
        _service = service;
    }

    [HttpPost]
    public Task<WorkflowTeamDto> CreateAsync(WorkflowTeamCreateOrUpdateDto input)
    {
        return _service.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _service.DeleteAsync(id);
    }

    [HttpDelete("{id}/role-scopes/{roleName}")]
    public Task DeleteRoleScopeAsync(Guid id, string roleName)
    {
        return _service.DeleteRoleScopeAsync(id, roleName);
    }

    [HttpGet("assignable-definitions")]
    public Task<ListResultDto<WorkflowDefinitionBasicDto>> GetAssignableDefinitionAsync(WorkflowDefinitionListRequestDto input)
    {
        return _service.GetAssignableDefinitionAsync(input);
    }

    [HttpGet("assignable-roles")]
    public Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
    {
        return _service.GetAssignableRolesAsync();
    }

    [HttpGet("assignable-users")]
    public Task<ListResultDto<IdentityUserDto>> GetAssignableUsersAsync(WorkflowTeamAssignableUserListRequestDto input)
    {
        return _service.GetAssignableUsersAsync(input);
    }

    [HttpGet("{id}")]
    public Task<WorkflowTeamDto> GetAsync(Guid id)
    {
        return _service.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<WorkflowTeamBasicDto>> GetListAsync(WorkflowTeamListRequestDto input)
    {
        return _service.GetListAsync(input);
    }

    [HttpGet("{id}/role-scopes")]
    public Task<ListResultDto<WorkflowTeamRoleScopeDto>> GetRoleScopesAsync(Guid id)
    {
        return _service.GetRoleScopesAsync(id);
    }

    [HttpPut("{id}/role-scopes")]
    public Task<ListResultDto<WorkflowTeamRoleScopeDto>> SetRoleScopeAsync(Guid id, WorkflowTeamRoleScopeCreateOrUpdateDto input)
    {
        return _service.SetRoleScopeAsync(id, input);
    }

    [HttpPut("{id}/users")]
    public Task SetUsersAsync(Guid id, WorkflowTeamUserUpdateRequestDto input)
    {
        return _service.SetUsersAsync(id, input);
    }

    [HttpPut("{id}")]
    public Task<WorkflowTeamDto> UpdateAsync(Guid id, WorkflowTeamCreateOrUpdateDto input)
    {
        return _service.UpdateAsync(id, input);
    }
}
