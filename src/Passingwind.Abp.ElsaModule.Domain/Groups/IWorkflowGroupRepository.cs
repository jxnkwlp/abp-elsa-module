using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Groups;

public interface IWorkflowGroupRepository : IRepository<WorkflowGroup, Guid>
{
    Task<long> GetCountAsync(string filter, CancellationToken cancellationToken = default);

    Task<List<WorkflowGroup>> GetListAsync(string filter, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<List<WorkflowGroup>> GetPagedListAsync(int skipCount, int maxResultCount, string filter, string sorting = null, bool includeDetails = false, CancellationToken cancellationToken = default);

}
