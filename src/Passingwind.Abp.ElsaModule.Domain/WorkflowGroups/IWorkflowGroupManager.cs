using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

public interface IWorkflowGroupManager : IDomainService
{
    Task EnsureWorkflowGroupPermissionDefinitionsAsync(CancellationToken cancellationToken = default);

    Task InitialWorkflowPermissionDefinitionsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Guid>> GetGrantedWorkflowIdsAsync(WorkflowGroup workflowGroup, CancellationToken cancellationToken = default);

    Task<IEnumerable<WorkflowGroup>> GetListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowGroup>> GetListByWorkflowIdAsync(Guid workflowId, CancellationToken cancellationToken = default);

    Task SetPermissionGrantsAsync(WorkflowGroup workflowGroup, IEnumerable<Guid> workflowIds, CancellationToken cancellationToken = default);

    Task SetPermissionGrantAsync(WorkflowGroup workflowGroup, Guid workflowId, bool isGrant, CancellationToken cancellationToken = default);

}
