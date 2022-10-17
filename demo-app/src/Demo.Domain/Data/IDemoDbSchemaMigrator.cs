using System.Threading.Tasks;

namespace Demo.Data;

public interface IDemoDbSchemaMigrator
{
    Task MigrateAsync();
}
