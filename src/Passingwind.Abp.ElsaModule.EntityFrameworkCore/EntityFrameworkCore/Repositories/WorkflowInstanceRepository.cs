using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class WorkflowInstanceRepository : EfCoreRepository<ElsaModuleDbContext, WorkflowInstance, Guid>, IWorkflowInstanceRepository
{
    public WorkflowInstanceRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> LongCountAsync(
        string name = null,
        IEnumerable<Guid> definitionIds = null,
        IEnumerable<Guid> definitionVersionIds = null,
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
        var dbset = await GetDbSetAsync();
        return await dbset
            .WhereIf(definitionIds?.Any()==true, x => definitionIds.Contains( x.WorkflowDefinitionId))
            .WhereIf(definitionVersionIds?.Any() == true, x => definitionVersionIds.Contains(x.WorkflowDefinitionVersionId))
            .WhereIf(version.HasValue, x => x.Version == version)
            .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
            .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(!string.IsNullOrEmpty(name), x => EF.Functions.Like(x.Name, $"%{name}%"))
            .WhereIf(creationTimes?.Length == 2, x => x.CreationTime >= creationTimes[0] && x.CreationTime <= creationTimes[1])
            .WhereIf(lastExecutedTimes?.Length == 2, x => x.LastExecutedTime.HasValue && x.LastExecutedTime >= lastExecutedTimes[0] && x.LastExecutedTime <= lastExecutedTimes[1])
            .WhereIf(finishedTimes?.Length == 2, x => x.FinishedTime.HasValue && x.FinishedTime >= finishedTimes[0] && x.FinishedTime <= finishedTimes[1])
            .WhereIf(cancelledTimes?.Length == 2, x => x.CancelledTime.HasValue && x.CancelledTime >= cancelledTimes[0] && x.CancelledTime <= cancelledTimes[1])
            .WhereIf(faultedTimes?.Length == 2, x => x.FaultedTime.HasValue && x.FaultedTime >= faultedTimes[0] && x.FaultedTime <= faultedTimes[1])
            .LongCountAsync(cancellationToken);
    }

    public async Task<List<WorkflowInstance>> GetListAsync(
        string name = null,
        IEnumerable<Guid> definitionIds = null,
        IEnumerable<Guid> definitionVersionIds = null,
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
        var dbset = await GetDbSetAsync();
        return await dbset
            .WhereIf(definitionIds?.Any() == true, x => definitionIds.Contains(x.WorkflowDefinitionId))
            .WhereIf(definitionVersionIds?.Any() == true, x => definitionVersionIds.Contains(x.WorkflowDefinitionVersionId))
            .WhereIf(version.HasValue, x => x.Version == version)
            .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
            .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(!string.IsNullOrEmpty(name), x => EF.Functions.Like(x.Name, $"%{name}%"))
            .WhereIf(creationTimes?.Length == 2, x => x.CreationTime >= creationTimes[0] && x.CreationTime <= creationTimes[1])
            .WhereIf(lastExecutedTimes?.Length == 2, x => x.LastExecutedTime.HasValue && x.LastExecutedTime >= lastExecutedTimes[0] && x.LastExecutedTime <= lastExecutedTimes[1])
            .WhereIf(finishedTimes?.Length == 2, x => x.FinishedTime.HasValue && x.FinishedTime >= finishedTimes[0] && x.FinishedTime <= finishedTimes[1])
            .WhereIf(cancelledTimes?.Length == 2, x => x.CancelledTime.HasValue && x.CancelledTime >= cancelledTimes[0] && x.CancelledTime <= cancelledTimes[1])
            .WhereIf(faultedTimes?.Length == 2, x => x.FaultedTime.HasValue && x.FaultedTime >= faultedTimes[0] && x.FaultedTime <= faultedTimes[1])
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkflowInstance>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        string name = null,
        IEnumerable<Guid> definitionIds = null,
        IEnumerable<Guid> definitionVersionIds = null,
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
        var dbset = await GetDbSetAsync();
        return await dbset
            .WhereIf(definitionIds?.Any() == true, x => definitionIds.Contains(x.WorkflowDefinitionId))
            .WhereIf(definitionVersionIds?.Any() == true, x => definitionVersionIds.Contains(x.WorkflowDefinitionVersionId))
            .WhereIf(version.HasValue, x => x.Version == version)
            .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
            .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(!string.IsNullOrEmpty(name), x => EF.Functions.Like(x.Name, $"%{name}%"))
            .WhereIf(creationTimes?.Length == 2, x => x.CreationTime >= creationTimes[0] && x.CreationTime <= creationTimes[1])
            .WhereIf(lastExecutedTimes?.Length == 2, x => x.LastExecutedTime.HasValue && x.LastExecutedTime >= lastExecutedTimes[0] && x.LastExecutedTime <= lastExecutedTimes[1])
            .WhereIf(finishedTimes?.Length == 2, x => x.FinishedTime.HasValue && x.FinishedTime >= finishedTimes[0] && x.FinishedTime <= finishedTimes[1])
            .WhereIf(cancelledTimes?.Length == 2, x => x.CancelledTime.HasValue && x.CancelledTime >= cancelledTimes[0] && x.CancelledTime <= cancelledTimes[1])
            .WhereIf(faultedTimes?.Length == 2, x => x.FaultedTime.HasValue && x.FaultedTime >= faultedTimes[0] && x.FaultedTime <= faultedTimes[1])
            .OrderBy(sorting ?? nameof(WorkflowInstance.CreationTime) + " desc")
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<DateTime, int>> GetStatusDateCountStatisticsAsync(WorkflowInstanceStatus workflowStatus, DateTime startDate, DateTime endDate, double timeZone = 0, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        var list = await dbset
              .Where(x => x.WorkflowStatus == workflowStatus && x.CreationTime >= startDate && x.CreationTime <= endDate)
              .Select(x => new { x.Id, x.CreationTime })
              .ToListAsync(cancellationToken);

        return list
                .Select(x => new { Id = x.Id, CreationTime = x.CreationTime.AddHours(timeZone) })
                .GroupBy(x => x.CreationTime.Date)
                .ToDictionary(x => x.Key.Date, x => x.Count());
    }

    public async Task<Dictionary<WorkflowInstanceStatus, int>> GetWorkflowStatusStatisticsAsync(
        string name = null,
        IEnumerable<Guid> definitionIds = null,
        IEnumerable<Guid> definitionVersionIds = null,
        int? version = null,
        string correlationId = null,
        DateTime[] creationTimes = null,
        CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        var list = await dbset
            .WhereIf(definitionIds?.Any() == true, x => definitionIds.Contains(x.WorkflowDefinitionId))
            .WhereIf(definitionVersionIds?.Any() == true, x => definitionVersionIds.Contains(x.WorkflowDefinitionVersionId))
            .WhereIf(version.HasValue, x => x.Version == version)
            .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(!string.IsNullOrEmpty(name), x => EF.Functions.Like(x.Name, $"%{name}%"))
            .WhereIf(creationTimes?.Length == 2, x => x.CreationTime >= creationTimes[0] && x.CreationTime <= creationTimes[1])
            .Select(x => new { x.Id, x.WorkflowStatus })
            .ToListAsync(cancellationToken);

        return list
                .GroupBy(x => x.WorkflowStatus)
                .ToDictionary(x => x.Key, x => x.Count());
    }
}
