using Volo.Abp.Modularity;

namespace Passingwind.WorkflowApp;

[DependsOn(
    typeof(WorkflowAppApplicationModule),
    typeof(WorkflowAppDomainTestModule)
    )]
public class WorkflowAppApplicationTestModule : AbpModule
{

}
