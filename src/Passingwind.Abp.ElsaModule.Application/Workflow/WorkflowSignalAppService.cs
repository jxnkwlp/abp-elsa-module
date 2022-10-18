using System;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Activities.Signaling.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    [AllowAnonymous]
    public class WorkflowSignalAppService : ElsaModuleAppService, IWorkflowSignalAppService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISignaler _signaler;

        public WorkflowSignalAppService(IHttpContextAccessor httpContextAccessor, ISignaler signaler)
        {
            _httpContextAccessor = httpContextAccessor;
            _signaler = signaler;
        }

        public async Task<WorkflowSignalDispatchResultDto> DispatchAsync(string signalName, WorkflowSignalDispatchRequestDto input)
        {
            var result = await _signaler.DispatchSignalAsync(signalName, input.Input, input.WorkflowInstanceId?.ToString(), input.CorrelationId);

            if (_httpContextAccessor.HttpContext.Response.HasStarted)
                return default;

            return new WorkflowSignalDispatchResultDto()
            {
                StartedWorkflows = result.Select(x => new WorkflowSignalResultDto
                {
                    ActivityId = string.IsNullOrEmpty(x.ActivityId) ? null : Guid.Parse(x.ActivityId),
                    WorkflowInstanceId = Guid.Parse(x.WorkflowInstanceId),
                }).ToList()
            };
        }

        public async Task<WorkflowSignalExecuteResultDto> ExecuteAsync(string signalName, WorkflowSignalExecuteRequestDto input)
        {
            var result = await _signaler.TriggerSignalAsync(signalName, input.Input, input.WorkflowInstanceId?.ToString(), input.CorrelationId);

            if (_httpContextAccessor.HttpContext.Response.HasStarted)
                return default;

            return new WorkflowSignalExecuteResultDto()
            {
                StartedWorkflows = result.Select(x => new WorkflowSignalResultDto
                {
                    ActivityId = string.IsNullOrEmpty(x.ActivityId) ? null : Guid.Parse(x.ActivityId),
                    WorkflowInstanceId = Guid.Parse(x.WorkflowInstanceId),
                }).ToList()
            };
        }
    }
}
