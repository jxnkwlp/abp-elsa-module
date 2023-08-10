using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowAuthorizationHandler : AuthorizationHandler<WorkflowAuthorizationRequirement, WorkflowDefinition>, ITransientDependency
{
    private readonly WorkflowPermissionProvider _workflowPermissionProvider;
    private readonly IOptions<PermissionManagementOptions> _permissionManagementOptions;
    private readonly IPermissionStore _permissionStore;

    public WorkflowAuthorizationHandler(WorkflowPermissionProvider workflowPermissionProvider, IOptions<PermissionManagementOptions> permissionManagementOptions, IPermissionStore permissionStore)
    {
        _workflowPermissionProvider = workflowPermissionProvider;
        _permissionManagementOptions = permissionManagementOptions;
        _permissionStore = permissionStore;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkflowAuthorizationRequirement requirement, WorkflowDefinition resource)
    {
        var userId = context.User.FindUserId();

        var granted = await _workflowPermissionProvider.IsGrantedAsync(context.User, requirement.Id, requirement.Name);

        if (granted)
        {
            context.Succeed(requirement);
        }
    }
}
