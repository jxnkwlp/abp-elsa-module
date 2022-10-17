using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Demo.Data;

/* This is used if database provider does't define
 * IDemoDbSchemaMigrator implementation.
 */
public class NullDemoDbSchemaMigrator : IDemoDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
