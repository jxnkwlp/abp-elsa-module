using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.Teams;

public interface IWorkflowTeamManager : IDomainService
{
    Task EnsurePermissionDefinitionsAsync(CancellationToken cancellationToken = default);

    Task InitialPermissionDefinitionsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetGrantedWorkflowIdsAsync(WorkflowTeam workflowTeam, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkflowTeam>> GetListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkflowTeam>> GetListByWorkflowIdAsync(Guid workflowId, CancellationToken cancellationToken = default);

    Task UpdatePermissionGrantsAsync(WorkflowTeam workflowTeam, CancellationToken cancellationToken = default);
}
