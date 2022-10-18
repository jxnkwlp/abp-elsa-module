using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Common
{
    [Authorize]
    public class WorkflowExecutionLogAppService : ElsaModuleAppService, IWorkflowExecutionLogAppService
    {
        private readonly IWorkflowExecutionLogRepository _workflowExecutionLogRepository;

        public WorkflowExecutionLogAppService(IWorkflowExecutionLogRepository workflowExecutionLogRepository)
        {
            _workflowExecutionLogRepository = workflowExecutionLogRepository;
        }

        public async Task<WorkflowExecutionLogDto> GetAsync(Guid id)
        {
            var entity = await _workflowExecutionLogRepository.GetAsync(id);

            return ObjectMapper.Map<WorkflowExecutionLog, WorkflowExecutionLogDto>(entity);
        }

        public async Task<PagedResultDto<WorkflowExecutionLogDto>> GetListAsync(WorkflowExecutionLogListRequestDto input)
        {
            var count = await _workflowExecutionLogRepository.GetCountAsync(workflowInstanceId: input.WorkflowInstanceId, activityId: input.ActivityId);
            var list = await _workflowExecutionLogRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, null, workflowInstanceId: input.WorkflowInstanceId, activityId: input.ActivityId);

            return new PagedResultDto<WorkflowExecutionLogDto>(count, ObjectMapper.Map<List<WorkflowExecutionLog>, List<WorkflowExecutionLogDto>>(list));
        }
    }
}
