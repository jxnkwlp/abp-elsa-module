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

        Task DeleteAsync(Guid id);
        Task BatchDeleteAsync(WorkflowInstancesBatchActionRequestDto input);

        Task CancelAsync(Guid id);
        Task BatchCancelAsync(WorkflowInstancesBatchActionRequestDto input);

        Task RetryAsync(Guid id, WorkflowInstanceRetryRequestDto input);
        Task BatchRetryAsync(WorkflowInstancesBatchActionRequestDto input);

        Task DispatchAsync(Guid id, WorkflowInstanceDispatchRequestDto input);
        Task ExecuteAsync(Guid id, WorkflowInstanceExecuteRequestDto input);

        Task<PagedResultDto<WorkflowInstanceBasicDto>> GetListAsync(WorkflowInstanceListRequestDto input);

        Task<ListResultDto<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid id);

        Task<WorkflowInstanceExecutionLogSummaryDto> GetLogSummaryAsync(Guid id);

        Task<WorkflowInstanceDateCountStatisticsResultDto> GetStatusDateCountStatisticsAsync(GetStatusDateCountStatisticsRequestDto input);

        Task<WorkflowInstanceStatusCountStatisticsResultDto> GetStatusCountStatisticsAsync();

    }
}
