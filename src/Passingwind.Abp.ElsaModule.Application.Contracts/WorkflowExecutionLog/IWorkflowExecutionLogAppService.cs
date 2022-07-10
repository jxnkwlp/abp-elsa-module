using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.Common
{
    public interface IWorkflowExecutionLogAppService : IApplicationService
    {
        Task<WorkflowExecutionLogDto> GetAsync(Guid id);
        Task<PagedResultDto<WorkflowExecutionLogDto>> GetListAsync(WorkflowExecutionLogListRequestDto input);
    }
}