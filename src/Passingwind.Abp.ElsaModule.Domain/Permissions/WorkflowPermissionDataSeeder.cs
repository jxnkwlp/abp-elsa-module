using System;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowPermissionDataSeeder : IWorkflowPermissionDataSeeder
{
    protected ICurrentTenant CurrentTenant { get; }
    protected IWorkflowPermissionService WorkflowPermissionService { get; }

    public WorkflowPermissionDataSeeder(ICurrentTenant currentTenant, IWorkflowPermissionService workflowPermissionService)
    {
        CurrentTenant = currentTenant;
        WorkflowPermissionService = workflowPermissionService;
    }

    public virtual async Task SendAsync(Guid? tenantId = null)
    {
        using (CurrentTenant.Change(tenantId))
        {
            await WorkflowPermissionService.EnsureWorkflowGroupPermissionDefinitionsAsync();

            await WorkflowPermissionService.InitialWorkflowPermissionDefinitionsAsync();
        }
    }
}
