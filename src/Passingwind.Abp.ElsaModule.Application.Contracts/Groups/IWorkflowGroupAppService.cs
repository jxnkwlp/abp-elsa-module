using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.Groups;

public interface IWorkflowGroupAppService : IApplicationService
{
    Task<PagedResultDto<WorkflowGroupDto>> GetListAsync(WorkflowGroupListRequestDto input);

    Task<WorkflowGroupDto> GetAsync(Guid id);

    Task<WorkflowGroupDto> CreateAsync(WorkflowGroupCreateOrUpdateDto input);

    Task<WorkflowGroupDto> UpdateAsync(Guid id, WorkflowGroupCreateOrUpdateDto input);

    Task DeleteAsync(Guid id);
}
