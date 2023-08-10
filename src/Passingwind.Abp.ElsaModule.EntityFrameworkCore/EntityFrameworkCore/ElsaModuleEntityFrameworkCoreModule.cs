using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Permissions;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

[DependsOn(
    typeof(ElsaModuleDomainModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class ElsaModuleEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ElsaModuleDbContext>(options =>
        {
            options.AddDefaultRepositories();

            options.Entity<WorkflowDefinitionVersion>(x => x.DefaultWithDetailsFunc = q => q.Include(c => c.Activities).Include(c => c.Connections));
            options.Entity<WorkflowInstance>(x => x.DefaultWithDetailsFunc = q => q
                                                                                .Include(c => c.ActivityData)
                                                                                .Include(c => c.ActivityScopes)
                                                                                .Include(c => c.BlockingActivities)
                                                                                .Include(c => c.ScheduledActivities)
                                                                                .Include(c => c.Metadata)
                                                                                .Include(c => c.Variables)
                                                                                .Include(c => c.Faults)
                                                                                );

            options.Entity<WorkflowTeam>(x => x.DefaultWithDetailsFunc = q => q
                                                                            .Include(x => x.Users)
                                                                            .Include(x => x.RoleScopes));
        });

        context.Services.TryAddTransient(typeof(IPermissionGroupDefinitionRepository), typeof(PermissionGroupDefinitionRepository));
        context.Services.TryAddTransient(typeof(IPermissionDefinitionRepository), typeof(PermissionDefinitionRepository));
    }

    #region Replace permission repository

    public class PermissionGroupDefinitionRepository : EfCorePermissionGroupDefinitionRecordRepository, IPermissionGroupDefinitionRepository
    {
        public PermissionGroupDefinitionRepository(IDbContextProvider<IPermissionManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }

    public class PermissionDefinitionRepository : EfCorePermissionDefinitionRecordRepository, IPermissionDefinitionRepository
    {
        public PermissionDefinitionRepository(IDbContextProvider<IPermissionManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }

    #endregion

}
