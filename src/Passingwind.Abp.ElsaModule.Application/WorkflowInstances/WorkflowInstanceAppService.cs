using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.WorkflowInstances;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Json;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowInstanceAppService : ElsaModuleAppService, IWorkflowInstanceAppService
    {
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IWorkflowExecutionLogRepository _workflowExecutionLogRepository;
        private readonly IJsonSerializer _jsonSerializer;

        public WorkflowInstanceAppService(IWorkflowInstanceRepository workflowInstanceRepository, IWorkflowExecutionLogRepository workflowExecutionLogRepository, IJsonSerializer jsonSerializer)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
            _workflowExecutionLogRepository = workflowExecutionLogRepository;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<WorkflowInstanceDto> GetAsync(Guid id)
        {
            var entity = await _workflowInstanceRepository.GetAsync(id);

            return ObjectMapper.Map<WorkflowInstance, WorkflowInstanceDto>(entity);
        }

        public async Task<ListResultDto<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid id)
        {
            var list = await _workflowExecutionLogRepository.GetListAsync(workflowInstanceId: id);

            return new ListResultDto<WorkflowExecutionLogDto>(ObjectMapper.Map<List<WorkflowExecutionLog>, List<WorkflowExecutionLogDto>>(list));
        }

        public async Task<PagedResultDto<WorkflowInstanceBasicDto>> GetListAsync(WorkflowInstanceListRequestDto input)
        {
            var count = await _workflowInstanceRepository.GetCountAsync(name: input.Name, version: input.Version, status: input.WorkflowStatus, correlationId: input.CorrelationId);
            var list = await _workflowInstanceRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, null, name: input.Name, version: input.Version, status: input.WorkflowStatus, correlationId: input.CorrelationId);

            return new PagedResultDto<WorkflowInstanceBasicDto>(count, ObjectMapper.Map<List<WorkflowInstance>, List<WorkflowInstanceBasicDto>>(list));
        }
    }
}
