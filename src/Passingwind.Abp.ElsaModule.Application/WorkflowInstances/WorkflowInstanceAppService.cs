using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elsa.Services;
using Passingwind.Abp.ElsaModule.Stores;
using Passingwind.Abp.ElsaModule.WorkflowInstances;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Json;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowInstanceAppService : ElsaModuleAppService, IWorkflowInstanceAppService
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IWorkflowExecutionLogRepository _workflowExecutionLogRepository;
        private readonly IStoreMapper _storeMapper;
        private readonly IWorkflowInstanceCanceller _workflowInstanceCanceller;
        private readonly IWorkflowReviver _workflowReviver;
        private readonly IWorkflowLaunchpad _workflowLaunchpad;

        public WorkflowInstanceAppService(IJsonSerializer jsonSerializer, IWorkflowInstanceRepository workflowInstanceRepository, IWorkflowExecutionLogRepository workflowExecutionLogRepository, IStoreMapper storeMapper, IWorkflowInstanceCanceller workflowInstanceCanceller, IWorkflowReviver workflowReviver, IWorkflowLaunchpad workflowLaunchpad)
        {
            _jsonSerializer = jsonSerializer;
            _workflowInstanceRepository = workflowInstanceRepository;
            _workflowExecutionLogRepository = workflowExecutionLogRepository;
            _storeMapper = storeMapper;
            _workflowInstanceCanceller = workflowInstanceCanceller;
            _workflowReviver = workflowReviver;
            _workflowLaunchpad = workflowLaunchpad;
        }

        public async Task CancelAsync(Guid id)
        {
            var entity = await _workflowInstanceRepository.GetAsync(id);

            if (entity.WorkflowStatus == Elsa.Models.WorkflowStatus.Idle || entity.WorkflowStatus == Elsa.Models.WorkflowStatus.Running || entity.WorkflowStatus == Elsa.Models.WorkflowStatus.Suspended)
            {
                var result = await _workflowInstanceCanceller.CancelAsync(id.ToString());

                if (result.Status == CancelWorkflowInstanceResultStatus.InvalidStatus)
                {
                    throw new UserFriendlyException($"Cannot cancel a workflow instance with status {result.WorkflowInstance!.WorkflowStatus}");
                }
            }
            else
                throw new UserFriendlyException($"Cannot cancel a workflow instance with status {entity.WorkflowStatus}");
        }

        public async Task RetryAsync(Guid id, WorkflowInstanceRetryRequestDto input)
        {
            var entity = await _workflowInstanceRepository.GetAsync(id);

            var instance = _storeMapper.MapToModel(entity);

            if (input.RunImmediately == false)
            {
                var workflowInstance = await _workflowReviver.ReviveAndQueueAsync(instance);
            }
            else
            {
                var result = await _workflowReviver.ReviveAndRunAsync(instance);

                if (result.WorkflowInstance.WorkflowStatus == Elsa.Models.WorkflowStatus.Faulted)
                {
                    throw new UserFriendlyException($"Workflow instance {result.WorkflowInstance.Id} has faulted");
                }
            }
        }

        public async Task DispatchAsync(Guid id, WorkflowInstanceDispatchRequestDto input)
        {
            var entity = await _workflowInstanceRepository.GetAsync(id);
            // var instance = _storeMapper.MapToModel(entity);

            await _workflowLaunchpad.DispatchPendingWorkflowAsync(id.ToString(), input.ActivityId?.ToString(), input.Input);
        }

        public async Task ExecuteAsync(Guid id, WorkflowInstanceExecuteRequestDto input)
        {
            var entity = await _workflowInstanceRepository.GetAsync(id);

            await _workflowLaunchpad.ExecutePendingWorkflowAsync(id.ToString(), input.ActivityId?.ToString(), input.Input);
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
