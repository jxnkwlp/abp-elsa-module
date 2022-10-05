using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public interface IWorkflowSignalAppService : IApplicationService
    {
        Task<WorkflowSignalDispatchResultDto> DispatchAsync(string signalName, WorkflowSignalDispatchRequestDto input);

        Task<WorkflowSignalExecuteResultDto> ExecuteAsync(string signalName, WorkflowSignalExecuteRequestDto input);
    }
}
