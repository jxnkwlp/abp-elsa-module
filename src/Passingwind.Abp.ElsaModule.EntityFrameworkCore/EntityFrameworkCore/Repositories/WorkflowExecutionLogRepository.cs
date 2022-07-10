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

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories
{
    public class WorkflowExecutionLogRepository : EfCoreRepository<ElsaModuleDbContext, WorkflowExecutionLog, Guid>, IWorkflowExecutionLogRepository
    {
        public WorkflowExecutionLogRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(Guid? workflowInstanceId = null, Guid? activityId = null, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();
            return await dbset
                .WhereIf(workflowInstanceId.HasValue, x => x.WorkflowInstanceId == workflowInstanceId)
                .WhereIf(activityId.HasValue, x => x.ActivityId == activityId)
                .LongCountAsync(cancellationToken);
        }

        public async Task<List<WorkflowExecutionLog>> GetListAsync(Guid? workflowInstanceId = null, Guid? activityId = null, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();
            return await dbset
                .WhereIf(workflowInstanceId.HasValue, x => x.WorkflowInstanceId == workflowInstanceId)
                .WhereIf(activityId.HasValue, x => x.ActivityId == activityId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<WorkflowExecutionLog>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, Guid? workflowInstanceId = null, Guid? activityId = null, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();
            return await dbset
                .WhereIf(workflowInstanceId.HasValue, x => x.WorkflowInstanceId == workflowInstanceId)
                .WhereIf(activityId.HasValue, x => x.ActivityId == activityId)
                .OrderBy(sorting ?? nameof(WorkflowExecutionLog.CreationTime) + " desc")
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        }

    }
}
