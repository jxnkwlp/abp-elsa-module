using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

[DependsOn(
    typeof(ElsaModuleDomainModule),
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
        });

    }
}
