using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow;

[RemoteService]
[Route("api/elsa/workflow")]
public class WorkflowController : ElsaModuleController, IWorkflowAppService
{
    private readonly IWorkflowAppService _service;

    public WorkflowController(IWorkflowAppService service)
    {
        _service = service;
    }

    [HttpGet("providers")]
    public Task<ListResultDto<WorkflowProviderDescriptorDto>> GetProvidersAsync()
    {
        return _service.GetProvidersAsync();
    }

    [HttpGet("storage-providers")]
    public virtual Task<ListResultDto<WorkflowStorageProviderInfoDto>> GetStorageProvidersAsync()
    {
        return _service.GetStorageProvidersAsync();
    }
}
