using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Passingwind.Abp.ElsaModule.Workflow;

[Area(ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[RemoteService(Name = ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[Route("api/elsa/workflows/signals")]
public class WorkflowSignalController : ElsaModuleController, IWorkflowSignalAppService
{
    private readonly IWorkflowSignalAppService _service;

    public WorkflowSignalController(IWorkflowSignalAppService service)
    {
        _service = service;
    }

    [HttpPost("dispatch/{signalName}")]
    public Task<WorkflowSignalDispatchResultDto> DispatchAsync(string signalName, WorkflowSignalDispatchRequestDto input)
    {
        return _service.DispatchAsync(signalName, input);
    }

    [HttpPost("execute/{signalName}")]
    public Task<WorkflowSignalExecuteResultDto> ExecuteAsync(string signalName, WorkflowSignalExecuteRequestDto input)
    {
        return _service.ExecuteAsync(signalName, input);
    }
}
