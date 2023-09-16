using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Abp.ElsaModule.WorkflowExecutionLog;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

[RemoteService]
[Route("api/elsa/workflow/instances")]
public class WorkflowInstanceController : ElsaModuleController, IWorkflowInstanceAppService
{
    private readonly IWorkflowInstanceAppService _service;

    public WorkflowInstanceController(IWorkflowInstanceAppService service)
    {
        _service = service;
    }

    [HttpPost("cancel")]
    public Task BatchCancelAsync(WorkflowInstanceBatchActionRequestDto input)
    {
        return _service.BatchCancelAsync(input);
    }

    [HttpDelete]
    public Task BatchDeleteAsync([FromBody] WorkflowInstanceBatchActionRequestDto input)
    {
        return _service.BatchDeleteAsync(input);
    }

    [HttpPost("retry")]
    public Task BatchRetryAsync(WorkflowInstanceBatchActionRequestDto input)
    {
        return _service.BatchRetryAsync(input);
    }

    [HttpPost("{id}/cancel")]
    public Task CancelAsync(Guid id)
    {
        return _service.CancelAsync(id);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _service.DeleteAsync(id);
    }

    [HttpPost("{id}/dispatch")]
    public Task DispatchAsync(Guid id, WorkflowInstanceDispatchRequestDto input)
    {
        return _service.DispatchAsync(id, input);
    }

    [HttpPost("{id}/execute")]
    public Task ExecuteAsync(Guid id, WorkflowInstanceExecuteRequestDto input)
    {
        return _service.ExecuteAsync(id, input);
    }

    [HttpGet("{id}")]
    public virtual Task<WorkflowInstanceDto> GetAsync(Guid id)
    {
        return _service.GetAsync(id);
    }

    [HttpGet("{id}/basic")]
    public Task<WorkflowInstanceBasicDto> GetBasicAsync(Guid id)
    {
        return _service.GetBasicAsync(id);
    }

    [HttpGet("{id}/execution-logs")]
    public Task<ListResultDto<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid id)
    {
        return _service.GetExecutionLogsAsync(id);
    }

    [HttpGet("{id}/faults")]
    public Task<ListResultDto<WorkflowInstanceFaultDto>> GetFaultsAsync(Guid id)
    {
        return _service.GetFaultsAsync(id);
    }

    [HttpGet("faults/by-definition/{id}")]
    public Task<PagedResultDto<WorkflowInstanceFaultDto>> GetFaultsByWorkflowDefinitionAsync(Guid id, WorkflowInstanceFaultRequestDto input)
    {
        return _service.GetFaultsByWorkflowDefinitionAsync(id, input);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<WorkflowInstanceBasicDto>> GetListAsync(WorkflowInstanceListRequestDto input)
    {
        return _service.GetListAsync(input);
    }

    [HttpGet("{id}/execution-logs/summary")]
    public Task<WorkflowInstanceExecutionLogSummaryDto> GetLogSummaryAsync(Guid id)
    {
        return _service.GetLogSummaryAsync(id);
    }

    [HttpGet("statistics/status")]
    public Task<WorkflowInstanceStatusCountStatisticsResultDto> GetStatusCountStatisticsAsync(WorkflowInstanceStatusCountStatisticsRequestDto input)
    {
        return _service.GetStatusCountStatisticsAsync(input);
    }

    [HttpGet("statistics/count-date")]
    public Task<WorkflowInstanceDateCountStatisticsResultDto> GetStatusDateCountStatisticsAsync(WorkflowInstanceDateCountStatisticsRequestDto input)
    {
        return _service.GetStatusDateCountStatisticsAsync(input);
    }

    [HttpPost("{id}/retry")]
    public Task RetryAsync(Guid id, WorkflowInstanceRetryRequestDto input)
    {
        return _service.RetryAsync(id, input);
    }
}
