using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

public interface IWorkflowGroupAppService : IApplicationService
{
    Task<PagedResultDto<WorkflowGroupBasicDto>> GetListAsync(WorkflowGroupListRequestDto input);

    Task<WorkflowGroupDto> GetAsync(Guid id);

    Task<WorkflowGroupDto> CreateAsync(WorkflowGroupCreateOrUpdateDto input);

    Task<WorkflowGroupDto> UpdateAsync(Guid id, WorkflowGroupCreateOrUpdateDto input);

    Task SetUsersAsync(Guid id, WorkflowGroupUpdateUsersRequestDto input);

    Task SetWorkflowflowsAsync(Guid id, WorkflowGroupSetWorkflowRequestDto input);

    Task DeleteAsync(Guid id);
}
