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
            options.AddDefaultRepositories(true);

            options.Entity<WorkflowDefinitionVersion>(x => x.DefaultWithDetailsFunc = q => q.Include(c => c.Activities).Include(c => c.Connections));

        });

    }
}
