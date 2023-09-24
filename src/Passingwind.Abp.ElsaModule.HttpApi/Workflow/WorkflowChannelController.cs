using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow;

[Area(ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[RemoteService(Name = ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[Route("api/elsa/workflow/channels")]
public class WorkflowChannelController : ElsaModuleController, IWorkflowChannelAppService
{
    private readonly IWorkflowChannelAppService _service;

    public WorkflowChannelController(IWorkflowChannelAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<ListResultDto<string>> GetListAsync()
    {
        return _service.GetListAsync();
    }
}
