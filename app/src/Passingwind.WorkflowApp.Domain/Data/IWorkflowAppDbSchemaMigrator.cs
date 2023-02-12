using System.Threading.Tasks;

namespace Passingwind.WorkflowApp.Data;

public interface IWorkflowAppDbSchemaMigrator
{
    Task MigrateAsync();
}
