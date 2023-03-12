using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Services;
using Elsa.Services.WorkflowStorage;
using Microsoft.AspNetCore.Authorization;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Permissions;
using Passingwind.Abp.ElsaModule.Stores;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Json;
using WorkflowExecutionLog = Passingwind.Abp.ElsaModule.Common.WorkflowExecutionLog;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

[Authorize(Policy = ElsaModulePermissions.Instances.Default)]
public class WorkflowInstanceAppService : ElsaModuleAppService, IWorkflowInstanceAppService
{
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowExecutionLogRepository _workflowExecutionLogRepository;
    private readonly IStoreMapper _storeMapper;
    private readonly IWorkflowInstanceCanceller _workflowInstanceCanceller;
    private readonly IWorkflowReviver _workflowReviver;
    private readonly IWorkflowLaunchpad _workflowLaunchpad;
    private readonly IWorkflowStorageService _workflowStorageService;
    private readonly IDistributedCache<WorkflowInstanceDateCountStatisticsResultDto> _workflowInstanceDateCountStatisticsDistributedCache;
    private readonly IDistributedCache<WorkflowInstanceStatusCountStatisticsResultDto> _workflowInstanceStatusCountStatisticsDistributedCache;
    private readonly IWorkflowPermissionService _workflowPermissionService;

    public WorkflowInstanceAppService(
        IJsonSerializer jsonSerializer,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowExecutionLogRepository workflowExecutionLogRepository,
        IStoreMapper storeMapper,
        IWorkflowInstanceCanceller workflowInstanceCanceller,
        IWorkflowReviver workflowReviver,
        IWorkflowLaunchpad workflowLaunchpad,
        IWorkflowStorageService workflowStorageService,
        IDistributedCache<WorkflowInstanceDateCountStatisticsResultDto> workflowInstanceDateCountStatisticsDistributedCache,
        IDistributedCache<WorkflowInstanceStatusCountStatisticsResultDto> workflowInstanceStatusCountStatisticsDistributedCache,
        IWorkflowPermissionService workflowPermissionService)
    {
        _jsonSerializer = jsonSerializer;
        _workflowInstanceRepository = workflowInstanceRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowExecutionLogRepository = workflowExecutionLogRepository;
        _storeMapper = storeMapper;
        _workflowInstanceCanceller = workflowInstanceCanceller;
        _workflowReviver = workflowReviver;
        _workflowLaunchpad = workflowLaunchpad;
        _workflowStorageService = workflowStorageService;
        _workflowInstanceDateCountStatisticsDistributedCache = workflowInstanceDateCountStatisticsDistributedCache;
        _workflowInstanceStatusCountStatisticsDistributedCache = workflowInstanceStatusCountStatisticsDistributedCache;
        _workflowPermissionService = workflowPermissionService;
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Action)]
    public async Task CancelAsync(Guid id)
    {
        var entity = await _workflowInstanceRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Action);

        if (entity.WorkflowStatus == WorkflowInstanceStatus.Idle || entity.WorkflowStatus == WorkflowInstanceStatus.Running || entity.WorkflowStatus == WorkflowInstanceStatus.Suspended)
        {
            var result = await _workflowInstanceCanceller.CancelAsync(id.ToString());

            if (result.Status == CancelWorkflowInstanceResultStatus.InvalidStatus)
                throw new UserFriendlyException($"Cannot cancel a workflow instance with status {result.WorkflowInstance!.WorkflowStatus}");
        }
        else
            throw new UserFriendlyException($"Cannot cancel a workflow instance with status {entity.WorkflowStatus}");
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Action)]
    public virtual async Task RetryAsync(Guid id, WorkflowInstanceRetryRequestDto input)
    {
        var entity = await _workflowInstanceRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Action);

        var instance = _storeMapper.MapToModel(entity);

        if (input.RunImmediately == false)
        {
            await _workflowReviver.ReviveAndQueueAsync(instance);
        }
        else
        {
            var result = await _workflowReviver.ReviveAndRunAsync(instance);

            if (result.WorkflowInstance.WorkflowStatus == Elsa.Models.WorkflowStatus.Faulted)
                throw new UserFriendlyException($"Workflow instance {result.WorkflowInstance.Id} has faulted");
        }
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Action)]
    public virtual async Task DispatchAsync(Guid id, WorkflowInstanceDispatchRequestDto input)
    {
        var entity = await _workflowInstanceRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Action);

        await _workflowLaunchpad.DispatchPendingWorkflowAsync(id.ToString(), input.ActivityId?.ToString(), input.Input);
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Action)]
    public async Task ExecuteAsync(Guid id, WorkflowInstanceExecuteRequestDto input)
    {
        var entity = await _workflowInstanceRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Action);

        await _workflowLaunchpad.ExecutePendingWorkflowAsync(id.ToString(), input.ActivityId?.ToString(), input.Input);
    }

    public async Task<WorkflowInstanceDto> GetAsync(Guid id)
    {
        // check can read details data
        var fullDataAccess = await AuthorizationService.IsGrantedAsync(ElsaModulePermissions.Instances.Data);

        var entity = await _workflowInstanceRepository.GetAsync(id, fullDataAccess);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Default);

        var instance = _storeMapper.MapToModel(entity);

        // var input = await _workflowStorageService.LoadAsync(instance);

        var dto = ObjectMapper.Map<WorkflowInstance, WorkflowInstanceDto>(entity);

        return dto;
    }

    public async Task<WorkflowInstanceBasicDto> GetBasicAsync(Guid id)
    {
        var entity = await _workflowInstanceRepository.GetAsync(id, false);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Default);

        return ObjectMapper.Map<WorkflowInstance, WorkflowInstanceBasicDto>(entity);
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Data)]
    public async Task<ListResultDto<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid id)
    {
        var entity = await _workflowInstanceRepository.GetAsync(id, false);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Data);

        var list = await _workflowExecutionLogRepository.GetListAsync(workflowInstanceId: id);

        return new ListResultDto<WorkflowExecutionLogDto>(ObjectMapper.Map<List<WorkflowExecutionLog>, List<WorkflowExecutionLogDto>>(list));
    }

    public async Task<PagedResultDto<WorkflowInstanceBasicDto>> GetListAsync(WorkflowInstanceListRequestDto input)
    {
        var grantedResult = await _workflowPermissionService.GetGrantsAsync();

        // overwrite 
        List<Guid> definitionIds = new List<Guid>(grantedResult.WorkflowIds);

        if (input.WorkflowDefinitionId.HasValue)
        {
            if (!grantedResult.WorkflowIds.Contains(input.WorkflowDefinitionId.Value))
            {
                throw new AbpAuthorizationException();
            }
            else
            {
                definitionIds = new List<Guid>() { input.WorkflowDefinitionId.Value };
            }
        }

        definitionIds = definitionIds.Distinct().ToList();

        var count = await _workflowInstanceRepository.LongCountAsync(
            name: input.Name,
            definitionIds: definitionIds,
            version: input.Version,
            status: input.WorkflowStatus,
            correlationId: input.CorrelationId,
            creationTimes: input.CreationTimes,
            finishedTimes: input.FinishedTimes,
            faultedTimes: input.FaultedTimes,
            lastExecutedTimes: input.LastExecutedTimes);

        var list = await _workflowInstanceRepository.GetPagedListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            name: input.Name,
            definitionIds: definitionIds,
            version: input.Version,
            status: input.WorkflowStatus,
            correlationId: input.CorrelationId,
            creationTimes: input.CreationTimes,
            finishedTimes: input.FinishedTimes,
            faultedTimes: input.FaultedTimes,
            lastExecutedTimes: input.LastExecutedTimes);

        return new PagedResultDto<WorkflowInstanceBasicDto>(count, ObjectMapper.Map<List<WorkflowInstance>, List<WorkflowInstanceBasicDto>>(list));
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _workflowInstanceRepository.GetAsync(id, false);
        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Delete);

        await _workflowInstanceRepository.DeleteAsync(id);
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Delete)]
    public async Task BatchDeleteAsync(WorkflowInstancesBatchActionRequestDto input)
    {
        if (input?.Ids?.Any() == true)
        {
            foreach (var id in input.Ids)
            {
                await DeleteAsync(id);
            }
        }
    }

    public async Task<WorkflowInstanceExecutionLogSummaryDto> GetLogSummaryAsync(Guid id)
    {
        var fullDataAccess = await AuthorizationService.IsGrantedAsync(ElsaModulePermissions.Instances.Data);

        var entity = await _workflowInstanceRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Instances.Default);

        var allLogs = await _workflowExecutionLogRepository.GetListAsync(workflowInstanceId: id);

        var dto = new WorkflowInstanceExecutionLogSummaryDto()
        {
            Activities = new List<WorkflowInstanceExecutionLogSummaryActivityDto>()
        };

        var groupLogs = allLogs.GroupBy(x => x.ActivityId);

        foreach (var itemLogs in groupLogs)
        {
            var logs = itemLogs.OrderByDescending(x => x.Timestamp);

            var summary = new WorkflowInstanceExecutionLogSummaryActivityDto()
            {
                ActivityId = itemLogs.Key,
                ActivityType = logs.First().ActivityType,

                StartTime = logs.Last().Timestamp,
                EndTime = logs.First().Timestamp,
                ExecutedCount = logs.Count(),
                Duration = (logs.First().Timestamp - logs.Last().Timestamp).TotalMilliseconds,

                IsExecuted = logs.Any(x => x.EventName == "Executed"),
                IsExecuting = !logs.Any(x => x.EventName == "Executed") && !logs.Any(x => x.EventName == "Faulted"),
                IsFaulted = logs.Any(x => x.EventName == "Faulted"),

                Outcomes = logs.Where(x => x.Data?.ContainsKey("Outcomes") == true).SelectMany(x => x.Data.SafeGetValue<string, object, string[]>("Outcomes")).ToArray(),

                StateData = !fullDataAccess ? null : entity.ActivityData.FirstOrDefault(x => x.ActivityId == itemLogs.Key)?.Data ?? default,
                JournalData = !fullDataAccess ? null : logs.First().Data?.Where(x => x.Key != "Outcomes")?.ToDictionary(x => x.Key, x => x.Value),
                Message = !fullDataAccess ? null : logs.First().Message,
            };

            dto.Activities.Add(summary);
        }

        // reorder again
        dto.Activities = dto.Activities.OrderByDescending(x => x.StartTime).ToList();

        return dto;
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Statistic)]
    public async Task<WorkflowInstanceDateCountStatisticsResultDto> GetStatusDateCountStatisticsAsync(WorkflowInstanceGetStatusDateCountStatisticsRequestDto input)
    {
        var datePeriod = input.DatePeriod ?? 30;
        if (datePeriod <= 0)
            throw new ArgumentOutOfRangeException("datePeriod must > 0");

        double tz = input.Tz;

        var endDate = Clock.Now;
        var startDate = Clock.Now.Date.AddDays(-datePeriod);

        var dto = await _workflowInstanceDateCountStatisticsDistributedCache.GetOrAddAsync($"workflow:instance:status:datecount:statistics:{datePeriod}:tz:{tz}", async () =>
        {
            var finished = await _workflowInstanceRepository.GetStatusDateCountStatisticsAsync(WorkflowInstanceStatus.Finished, startDate, endDate, timeZone: tz);
            var faulted = await _workflowInstanceRepository.GetStatusDateCountStatisticsAsync(WorkflowInstanceStatus.Faulted, startDate, endDate, timeZone: tz);

            var dto = new WorkflowInstanceDateCountStatisticsResultDto();

            for (int i = 1; i <= datePeriod; i++)
            {
                var date = startDate.Date.AddDays(i);
                dto.Items.Add(new WorkflowInstanceDateCountStatisticDto
                {
                    Date = date,
                    FailedCount = faulted.ContainsKey(date.Date) ? faulted[date.Date] : 0,
                    FinishedCount = finished.ContainsKey(date.Date) ? finished[date.Date] : 0,
                });
            }

            return dto;
        }, () => new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10),
        });

        return dto;
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Statistic)]
    public async Task<WorkflowInstanceStatusCountStatisticsResultDto> GetStatusCountStatisticsAsync()
    {
        return await _workflowInstanceStatusCountStatisticsDistributedCache.GetOrAddAsync("workflow:instance:status:count:statistic", async () =>
               {
                   var allCount = await _workflowInstanceRepository.LongCountAsync();
                   var runningCount = await _workflowInstanceRepository.LongCountAsync(status: WorkflowInstanceStatus.Running);
                   var faultedCount = await _workflowInstanceRepository.LongCountAsync(status: WorkflowInstanceStatus.Faulted);
                   var suspendedCount = await _workflowInstanceRepository.LongCountAsync(status: WorkflowInstanceStatus.Suspended);
                   var finishedCount = await _workflowInstanceRepository.LongCountAsync(status: WorkflowInstanceStatus.Finished);

                   return new WorkflowInstanceStatusCountStatisticsResultDto
                   {
                       All = allCount,
                       Faulted = faultedCount,
                       Finished = finishedCount,
                       Suspended = suspendedCount,
                       Running = runningCount,
                   };
               }, () => new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
               {
                   AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(2),
               });
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Action)]
    public async Task BatchCancelAsync(WorkflowInstancesBatchActionRequestDto input)
    {
        // TODO check permissions

        if (input?.Ids?.Any() == true)
        {
            foreach (var id in input.Ids)
            {
                await CancelAsync(id);
            }
        }
    }

    [Authorize(Policy = ElsaModulePermissions.Instances.Action)]
    public async Task BatchRetryAsync(WorkflowInstancesBatchActionRequestDto input)
    {
        // TODO check permissions

        if (input?.Ids?.Any() == true)
        {
            foreach (var id in input.Ids)
            {
                await RetryAsync(id, new WorkflowInstanceRetryRequestDto { RunImmediately = false });
            }
        }
    }

}
