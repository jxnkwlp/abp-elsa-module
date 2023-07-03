using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Common;

public interface IWorkflowDefinitionVersionRepository : IRepository<WorkflowDefinitionVersion, Guid>
{
    Task<long> GetCountAsync(Expression<Func<WorkflowDefinitionVersion, bool>> expression, CancellationToken cancellationToken = default);

    Task<List<WorkflowDefinitionVersion>> GetPagedListAsync(int skipCount, int maxResultCount, Expression<Func<WorkflowDefinitionVersion, bool>> expression, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<List<WorkflowDefinitionVersion>> GetListAsync(IEnumerable<Guid> definitionIds = null, bool? isLatest = null, bool? isPublished = null, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<WorkflowDefinitionVersion> GetLatestAsync(Guid definitionId, bool includeDetails = true, CancellationToken cancellationToken = default);

    Task<WorkflowDefinitionVersion> GetByVersionAsync(Guid definitionId, int version, bool includeDetails = true, CancellationToken cancellationToken = default);

    Task<WorkflowDefinitionVersion> FindByVersionAsync(Guid definitionId, int version, bool includeDetails = true, CancellationToken cancellationToken = default);

    Task<int?> GetPreviousVersionNumberAsync(Guid definitionId, int version, CancellationToken cancellationToken = default);
}
