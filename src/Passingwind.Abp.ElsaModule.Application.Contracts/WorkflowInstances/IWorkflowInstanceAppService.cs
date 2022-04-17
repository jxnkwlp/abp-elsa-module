using System;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public interface IWorkflowInstanceAppService : IApplicationService
    {
        Task<WorkflowInstanceDto> GetAsync(Guid id);
        Task<PagedResultDto<WorkflowInstanceBasicDto>> GetListAsync(WorkflowInstanceListRequestDto input);
        Task<ListResultDto<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid id);
    }
}
