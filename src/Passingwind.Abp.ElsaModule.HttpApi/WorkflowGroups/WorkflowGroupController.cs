using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

[RemoteService]
[Route("api/elsa/workflow/groups")]
public class WorkflowGroupController : Controller, IWorkflowGroupAppService
{
    private readonly IWorkflowGroupAppService _service;

    public WorkflowGroupController(IWorkflowGroupAppService service)
    {
        _service = service;
    }

    [HttpPost]
    public Task<WorkflowGroupDto> CreateAsync(WorkflowGroupCreateOrUpdateDto input)
    {
        return _service.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _service.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<WorkflowGroupDto> GetAsync(Guid id)
    {
        return _service.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<WorkflowGroupBasicDto>> GetListAsync(WorkflowGroupListRequestDto input)
    {
        return _service.GetListAsync(input);
    }

    [HttpPut("{id}/users")]
    public Task SetUsersAsync(Guid id, WorkflowGroupUpdateUsersRequestDto input)
    {
        return _service.SetUsersAsync(id, input);
    }

    [HttpPut("{id}/workflows")]
    public Task SetWorkflowflowsAsync(Guid id, WorkflowGroupSetWorkflowRequestDto input)
    {
        return _service.SetWorkflowflowsAsync(id, input);
    }

    [HttpPut("{id}")]
    public Task<WorkflowGroupDto> UpdateAsync(Guid id, WorkflowGroupCreateOrUpdateDto input)
    {
        return _service.UpdateAsync(id, input);
    }
}
