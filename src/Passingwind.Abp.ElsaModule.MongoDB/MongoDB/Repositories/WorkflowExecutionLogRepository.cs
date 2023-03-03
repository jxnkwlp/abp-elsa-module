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

public class WorkflowExecutionLogRepository : MongoDbRepository<IElsaModuleMongoDbContext, WorkflowExecutionLog, Guid>, IWorkflowExecutionLogRepository
{
    public WorkflowExecutionLogRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> GetCountAsync(Guid? workflowInstanceId = null, Guid? activityId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(workflowInstanceId.HasValue, x => x.WorkflowInstanceId == workflowInstanceId)
            .WhereIf(activityId.HasValue, x => x.ActivityId == activityId)
            .As<IMongoQueryable<WorkflowExecutionLog>>()
            .LongCountAsync(cancellationToken);
    }

    public async Task<List<WorkflowExecutionLog>> GetListAsync(Guid? workflowInstanceId = null, Guid? activityId = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(workflowInstanceId.HasValue, x => x.WorkflowInstanceId == workflowInstanceId)
            .WhereIf(activityId.HasValue, x => x.ActivityId == activityId)
            .As<IMongoQueryable<WorkflowExecutionLog>>()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkflowExecutionLog>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, Guid? workflowInstanceId = null, Guid? activityId = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(workflowInstanceId.HasValue, x => x.WorkflowInstanceId == workflowInstanceId)
            .WhereIf(activityId.HasValue, x => x.ActivityId == activityId)
            .OrderBy(sorting ?? nameof(WorkflowExecutionLog.CreationTime) + " desc")
            .As<IMongoQueryable<WorkflowExecutionLog>>()
            .PageBy<WorkflowExecutionLog, IMongoQueryable<WorkflowExecutionLog>>(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

}
