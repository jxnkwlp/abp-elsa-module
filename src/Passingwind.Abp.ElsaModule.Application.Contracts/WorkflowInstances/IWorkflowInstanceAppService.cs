using System;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.WorkflowExecutionLog;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public interface IWorkflowInstanceAppService : IApplicationService
{
    Task<WorkflowInstanceBasicDto> GetBasicAsync(Guid id);
    Task<WorkflowInstanceDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);
    Task BatchDeleteAsync(WorkflowInstanceBatchActionRequestDto input);

    Task CancelAsync(Guid id);
    Task BatchCancelAsync(WorkflowInstanceBatchActionRequestDto input);

    Task RetryAsync(Guid id, WorkflowInstanceRetryRequestDto input);
    Task BatchRetryAsync(WorkflowInstanceBatchActionRequestDto input);

    Task DispatchAsync(Guid id, WorkflowInstanceDispatchRequestDto input);
    Task ExecuteAsync(Guid id, WorkflowInstanceExecuteRequestDto input);

    Task<PagedResultDto<WorkflowInstanceBasicDto>> GetListAsync(WorkflowInstanceListRequestDto input);

    Task<ListResultDto<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid id);

    Task<WorkflowInstanceExecutionLogSummaryDto> GetLogSummaryAsync(Guid id);

    Task<ListResultDto<WorkflowInstanceFaultDto>> GetFaultsAsync(Guid id);
    Task<PagedResultDto<WorkflowInstanceFaultDto>> GetFaultsByWorkflowDefinitionAsync(Guid id, WorkflowInstanceFaultRequestDto input);

    Task<WorkflowInstanceDateCountStatisticsResultDto> GetStatusDateCountStatisticsAsync(WorkflowInstanceDateCountStatisticsRequestDto input);

    Task<WorkflowInstanceStatusCountStatisticsResultDto> GetStatusCountStatisticsAsync(WorkflowInstanceStatusCountStatisticsRequestDto input);

}
