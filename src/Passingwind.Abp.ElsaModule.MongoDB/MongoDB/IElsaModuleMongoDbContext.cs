using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB;

[ConnectionStringName(ElsaModuleDbProperties.ConnectionStringName)]
public interface IElsaModuleMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
