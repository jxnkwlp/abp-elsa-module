using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Groups;

[RemoteService]
[Route("api/elsa/workflow/groups")]
public class WorkflowGroupController : ElsaModuleController, IWorkflowGroupAppService
{
    private readonly IWorkflowGroupAppService _service;

    public WorkflowGroupController(IWorkflowGroupAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<WorkflowGroupDto>> GetListAsync(WorkflowGroupListRequestDto input)
    {
        return _service.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public virtual Task<WorkflowGroupDto> GetAsync(Guid id)
    {
        return _service.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<WorkflowGroupDto> CreateAsync(WorkflowGroupCreateOrUpdateDto input)
    {
        return _service.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public virtual Task<WorkflowGroupDto> UpdateAsync(Guid id, WorkflowGroupCreateOrUpdateDto input)
    {
        return _service.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _service.DeleteAsync(id);
    }
}
