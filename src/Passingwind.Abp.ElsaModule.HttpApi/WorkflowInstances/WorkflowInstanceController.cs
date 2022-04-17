using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    [RemoteService]
    [Route("api/workflow-instances")]
    public class WorkflowInstanceController : ElsaModuleController, IWorkflowInstanceAppService
    {
        private readonly IWorkflowInstanceAppService _service;

        public WorkflowInstanceController(IWorkflowInstanceAppService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public virtual Task<WorkflowInstanceDto> GetAsync(Guid id)
        {
            return _service.GetAsync(id);
        }

        [HttpGet("{id}/execution-logs")]
        public Task<ListResultDto<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid id)
        {
            return _service.GetExecutionLogsAsync(id);
        }

        [HttpGet()]
        public virtual Task<PagedResultDto<WorkflowInstanceBasicDto>> GetListAsync(WorkflowInstanceListRequestDto input)
        {
            return _service.GetListAsync(input);
        }

    }
}
