using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Passingwind.Abp.ElsaModule.Permissions;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.PermissionManagement.MongoDB;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.MongoDB;

[DependsOn(
    typeof(ElsaModuleDomainModule),
    typeof(AbpPermissionManagementMongoDbModule),
    typeof(AbpMongoDbModule)
    )]
public class ElsaModuleMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<ElsaModuleMongoDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, MongoQuestionRepository>();
             */

            options.AddDefaultRepositories();
        });

        Configure<AbpUnitOfWorkDefaultOptions>(options => options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled);

        context.Services.TryAddTransient(typeof(IPermissionGroupDefinitionRepository), typeof(PermissionGroupDefinitionRepository));
        context.Services.TryAddTransient(typeof(IPermissionDefinitionRepository), typeof(PermissionDefinitionRepository));
    }

    public class PermissionGroupDefinitionRepository : MongoPermissionGroupDefinitionRecordRepository, IPermissionGroupDefinitionRepository
    {
        public PermissionGroupDefinitionRepository(IMongoDbContextProvider<IPermissionManagementMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }

    public class PermissionDefinitionRepository : MongoPermissionDefinitionRecordRepository, IPermissionDefinitionRepository
    {
        public PermissionDefinitionRepository(IMongoDbContextProvider<IPermissionManagementMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
