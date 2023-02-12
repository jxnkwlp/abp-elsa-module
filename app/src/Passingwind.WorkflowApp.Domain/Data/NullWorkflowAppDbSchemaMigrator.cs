using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.WorkflowApp.Data;

/* This is used if database provider does't define
 * IWorkflowAppDbSchemaMigrator implementation.
 */
public class NullWorkflowAppDbSchemaMigrator : IWorkflowAppDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
