using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowPermissionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IWorkflowPermissionDataSeeder _workflowPermissionDataSeeder;

    public WorkflowPermissionDataSeedContributor(IWorkflowPermissionDataSeeder workflowPermissionDataSeeder)
    {
        _workflowPermissionDataSeeder = workflowPermissionDataSeeder;
    }

    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await _workflowPermissionDataSeeder.SendAsync(context.TenantId);
    }
}