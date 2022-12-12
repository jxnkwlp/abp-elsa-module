using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class WorkflowInstanceRepository : MongoDbRepository<IElsaModuleMongoDbContext, WorkflowInstance, Guid>, IWorkflowInstanceRepository
{
    public WorkflowInstanceRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> LongCountAsync(
        string name = null,
        Guid? definitionId = null,
        Guid? definitionVersionId = null,
        int? version = null,
        WorkflowInstanceStatus? status = null,
        string correlationId = null,
        DateTime[] creationTimes = null,
        DateTime[] lastExecutedTimes = null,
        DateTime[] finishedTimes = null,
        DateTime[] cancelledTimes = null,
        DateTime[] faultedTimes = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
            .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
            .WhereIf(version.HasValue, x => x.Version == version)
            .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
            .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name))
            .WhereIf(creationTimes?.Length == 2, x => x.CreationTime >= creationTimes[0] && x.CreationTime <= creationTimes[1])
            .WhereIf(lastExecutedTimes?.Length == 2, x => x.LastExecutedTime.HasValue && x.LastExecutedTime >= lastExecutedTimes[0] && x.LastExecutedTime <= lastExecutedTimes[1])
            .WhereIf(finishedTimes?.Length == 2, x => x.FinishedTime.HasValue && x.FinishedTime >= finishedTimes[0] && x.FinishedTime <= finishedTimes[1])
            .WhereIf(cancelledTimes?.Length == 2, x => x.CancelledTime.HasValue && x.CancelledTime >= cancelledTimes[0] && x.CancelledTime <= cancelledTimes[1])
            .WhereIf(faultedTimes?.Length == 2, x => x.FaultedTime.HasValue && x.FaultedTime >= faultedTimes[0] && x.FaultedTime <= faultedTimes[1])
            .As<IMongoQueryable<WorkflowInstance>>()
            .LongCountAsync(cancellationToken);
    }

    public async Task<List<WorkflowInstance>> GetListAsync(
        string name = null,
        Guid? definitionId = null,
        Guid? definitionVersionId = null,
        int? version = null,
        WorkflowInstanceStatus? status = null,
        string correlationId = null,
        DateTime[] creationTimes = null,
        DateTime[] lastExecutedTimes = null,
        DateTime[] finishedTimes = null,
        DateTime[] cancelledTimes = null,
        DateTime[] faultedTimes = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
            .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
            .WhereIf(version.HasValue, x => x.Version == version)
            .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
            .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name))
            .WhereIf(creationTimes?.Length == 2, x => x.CreationTime >= creationTimes[0] && x.CreationTime <= creationTimes[1])
            .WhereIf(lastExecutedTimes?.Length == 2, x => x.LastExecutedTime.HasValue && x.LastExecutedTime >= lastExecutedTimes[0] && x.LastExecutedTime <= lastExecutedTimes[1])
            .WhereIf(finishedTimes?.Length == 2, x => x.FinishedTime.HasValue && x.FinishedTime >= finishedTimes[0] && x.FinishedTime <= finishedTimes[1])
            .WhereIf(cancelledTimes?.Length == 2, x => x.CancelledTime.HasValue && x.CancelledTime >= cancelledTimes[0] && x.CancelledTime <= cancelledTimes[1])
            .WhereIf(faultedTimes?.Length == 2, x => x.FaultedTime.HasValue && x.FaultedTime >= faultedTimes[0] && x.FaultedTime <= faultedTimes[1])
            .As<IMongoQueryable<WorkflowInstance>>()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkflowInstance>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        string name = null,
        Guid? definitionId = null,
        Guid? definitionVersionId = null,
        int? version = null,
        WorkflowInstanceStatus? status = null,
        string correlationId = null,
        DateTime[] creationTimes = null,
        DateTime[] lastExecutedTimes = null,
        DateTime[] finishedTimes = null,
        DateTime[] cancelledTimes = null,
        DateTime[] faultedTimes = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
            .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
            .WhereIf(version.HasValue, x => x.Version == version)
            .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
            .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name))
            .WhereIf(creationTimes?.Length == 2, x => x.CreationTime >= creationTimes[0] && x.CreationTime <= creationTimes[1])
            .WhereIf(lastExecutedTimes?.Length == 2, x => x.LastExecutedTime.HasValue && x.LastExecutedTime >= lastExecutedTimes[0] && x.LastExecutedTime <= lastExecutedTimes[1])
            .WhereIf(finishedTimes?.Length == 2, x => x.FinishedTime.HasValue && x.FinishedTime >= finishedTimes[0] && x.FinishedTime <= finishedTimes[1])
            .WhereIf(cancelledTimes?.Length == 2, x => x.CancelledTime.HasValue && x.CancelledTime >= cancelledTimes[0] && x.CancelledTime <= cancelledTimes[1])
            .WhereIf(faultedTimes?.Length == 2, x => x.FaultedTime.HasValue && x.FaultedTime >= faultedTimes[0] && x.FaultedTime <= faultedTimes[1])
            .OrderBy(sorting ?? nameof(WorkflowInstance.CreationTime) + " desc")
            .As<IMongoQueryable<WorkflowInstance>>()
            .PageBy<WorkflowInstance, IMongoQueryable<WorkflowInstance>>(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<DateTime, int>> GetStatusDateCountStatisticsAsync(WorkflowInstanceStatus workflowStatus, DateTime startDate, DateTime endDate, double timeZone = 0, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        var startDate2 = startDate.Date;
        var endDate2 = endDate.Date;

        var list = await query
                   .Where(x => x.WorkflowStatus == workflowStatus && x.CreationTime >= startDate2 && x.CreationTime <= endDate2)
                   .Select(x => new { x.Id, x.CreationTime })
                   .ToListAsync(cancellationToken);

        return list
                .Select(x => new { Id = x.Id, CreationTime = x.CreationTime.AddHours(timeZone) })
                .GroupBy(x => x.CreationTime.Date)
                .ToDictionary(x => x.Key.Date, x => x.Count());
    }

    public async Task<Dictionary<WorkflowInstanceStatus, int>> GetWorkflowStatusStatisticsAsync(string name = null, Guid? definitionId = null, Guid? definitionVersionId = null, int? version = null, string correlationId = null, DateTime[] creationTimes = null, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return query
            .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
            .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
            .WhereIf(version.HasValue, x => x.Version == version)
            .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name))
            .WhereIf(creationTimes?.Length == 2, x => x.CreationTime >= creationTimes[0] && x.CreationTime <= creationTimes[1])
            .GroupBy(x => x.WorkflowStatus)
            .ToDictionary(x => x.Key, x => x.Count());
    }
}
