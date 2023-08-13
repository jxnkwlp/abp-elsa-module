using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Passingwind.Abp.ElsaModule.Permissions;

public interface IWorkflowPermissionProvider : ITransientDependency
{
    Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, Guid workflowId, string name, CancellationToken cancellationToken = default);
    Task<bool> IsGrantedAsync(Guid workflowId, string name, CancellationToken cancellationToken = default);

    Task<WorkflowPermissionGrantResult> GetGrantsAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default);
    Task<WorkflowPermissionGrantResult> GetGrantsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<IdentityUser>> GetWorkflowOwnersAsync(WorkflowDefinition workflowDefinition, CancellationToken cancellationToken = default);
    Task AddWorkflowOwnerAsync(WorkflowDefinition workflowDefinition, IdentityUser user, CancellationToken cancellationToken = default);
    Task RemoveWorkflowOwnerAsync(WorkflowDefinition workflowDefinition, IdentityUser user, CancellationToken cancellationToken = default);

    Task SetUserWorkflowPermissionGrantAsync(WorkflowDefinition workflowDefinition, IdentityUser user, bool isGranted, CancellationToken cancellationToken = default);

    Task CreateWorkflowPermissionDefinitionsAsync(WorkflowDefinition workflowDefinition, CancellationToken cancellationToken = default);

    Task InitialWorkflowPermissionDefinitionsAsync(CancellationToken cancellationToken = default);

    Task RestoreWorkflowTeamPermissionGrantsAsync(CancellationToken cancellationToken = default);
}
