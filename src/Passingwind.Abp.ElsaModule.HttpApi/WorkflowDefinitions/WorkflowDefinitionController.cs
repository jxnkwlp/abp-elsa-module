using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

[RemoteService]
[Route("api/elsa/workflow/definitions")]
public class WorkflowDefinitionController : ElsaModuleController, IWorkflowDefinitionAppService
{
    private readonly IWorkflowDefinitionAppService _service;

    public WorkflowDefinitionController(IWorkflowDefinitionAppService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public virtual Task<WorkflowDefinitionVersionDto> GetAsync(Guid id)
    {
        return _service.GetAsync(id);
    }

    [HttpGet()]
    public virtual Task<PagedResultDto<WorkflowDefinitionDto>> GetListAsync(WorkflowDefinitionListRequestDto input)
    {
        return _service.GetListAsync(input);
    }

    [HttpPost()]
    public virtual Task<WorkflowDefinitionVersionDto> CreateAsync(WorkflowDefinitionVersionCreateOrUpdateDto input)
    {
        return _service.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public virtual Task<WorkflowDefinitionVersionDto> UpdateAsync(Guid id, WorkflowDefinitionVersionCreateOrUpdateDto input)
    {
        return _service.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _service.DeleteAsync(id);
    }

    [HttpDelete("{id}/versions/{version}")]
    public Task DeleteVersionAsync(Guid id, int version)
    {
        return _service.DeleteVersionAsync(id, version);
    }

    [HttpGet("{id}/definition")]
    public Task<WorkflowDefinitionDto> GetDefinitionAsync(Guid id)
    {
        return _service.GetDefinitionAsync(id);
    }

    [HttpGet("{id}/versions/{version}")]
    public Task<WorkflowDefinitionVersionDto> GetVersionAsync(Guid id, int version)
    {
        return _service.GetVersionAsync(id, version);
    }

    [HttpPut("{id}/definition")]
    public Task<WorkflowDefinitionDto> UpdateDefinitionAsync(Guid id, WorkflowDefinitionCreateOrUpdateDto input)
    {
        return _service.UpdateDefinitionAsync(id, input);
    }

    [HttpGet("{id}/versions")]
    public Task<PagedResultDto<WorkflowDefinitionVersionListItemDto>> GetVersionsAsync(Guid id, WorkflowDefinitionVersionListRequestDto input)
    {
        return _service.GetVersionsAsync(id, input);
    }

    [HttpPut("{id}/unpublish")]
    public Task UnPublishAsync(Guid id)
    {
        return _service.UnPublishAsync(id);
    }

    [HttpPut("{id}/publish")]
    public Task PublishAsync(Guid id)
    {
        return _service.PublishAsync(id);
    }

    [HttpPost("{id}/execute")]
    public Task ExecuteAsync(Guid id, WorkflowDefinitionExecuteRequestDto input)
    {
        return _service.ExecuteAsync(id, input);
    }

    [HttpPost("{id}/dispatch")]
    public Task<WorkflowDefinitionDispatchResultDto> DispatchAsync(Guid id, WorkflowDefinitionDispatchRequestDto input)
    {
        return _service.DispatchAsync(id, input);
    }

    [HttpGet("{id}/versions/{version}/previous-version")]
    public Task<WorkflowDefinitionVersionDto> GetPreviousVersionAsync(Guid id, int version)
    {
        return _service.GetPreviousVersionAsync(id, version);
    }

    [HttpPost("{id}/retract")]
    public Task RetractAsync(Guid id)
    {
        return _service.RetractAsync(id);
    }

    [HttpPost("{id}/revert")]
    public Task RevertAsync(Guid id, WorkflowDefinitionRevertRequestDto input)
    {
        return _service.RevertAsync(id, input);
    }

}
