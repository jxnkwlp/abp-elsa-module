using System;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Passingwind.Abp.ElsaModule.Teams;

public interface IWorkflowTeamAppService : IApplicationService
{
    Task<PagedResultDto<WorkflowTeamBasicDto>> GetListAsync(WorkflowTeamListRequestDto input);

    Task<WorkflowTeamDto> GetAsync(Guid id);

    Task<WorkflowTeamDto> CreateAsync(WorkflowTeamCreateOrUpdateDto input);

    Task<WorkflowTeamDto> UpdateAsync(Guid id, WorkflowTeamCreateOrUpdateDto input);

    Task SetUsersAsync(Guid id, WorkflowTeamUserUpdateRequestDto input);

    Task<ListResultDto<WorkflowTeamRoleScopeDto>> GetRoleScopesAsync(Guid id);

    Task<ListResultDto<WorkflowTeamRoleScopeDto>> SetRoleScopeAsync(Guid id, WorkflowTeamRoleScopeCreateOrUpdateDto input);

    Task DeleteRoleScopeAsync(Guid id, string roleName);

    Task DeleteAsync(Guid id);

    Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync();
    Task<ListResultDto<IdentityUserDto>> GetAssignableUsersAsync(WorkflowTeamAssignableUserListRequestDto input);
    Task<ListResultDto<WorkflowDefinitionBasicDto>> GetAssignableDefinitionAsync(WorkflowDefinitionListRequestDto input);
}
