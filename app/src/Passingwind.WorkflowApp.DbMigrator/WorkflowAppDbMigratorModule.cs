using Passingwind.WorkflowApp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Passingwind.WorkflowApp.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WorkflowAppEntityFrameworkCoreModule),
    typeof(WorkflowAppApplicationContractsModule)
    )]
public class WorkflowAppDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
