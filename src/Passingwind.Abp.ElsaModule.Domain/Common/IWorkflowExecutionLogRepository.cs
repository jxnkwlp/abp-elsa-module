using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Common;

public interface IWorkflowExecutionLogRepository : IRepository<WorkflowExecutionLog, Guid>
{
    Task<long> GetCountAsync(Guid? workflowInstanceId = null, Guid? activityId = null, CancellationToken cancellationToken = default);

    Task<List<WorkflowExecutionLog>> GetListAsync(Guid? workflowInstanceId = null, Guid? activityId = null, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<List<WorkflowExecutionLog>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, Guid? workflowInstanceId = null, Guid? activityId = null, bool includeDetails = false, CancellationToken cancellationToken = default);
}
