using Passingwind.WorkflowApp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Passingwind.WorkflowApp;

[DependsOn(
    typeof(WorkflowAppEntityFrameworkCoreTestModule)
    )]
public class WorkflowAppDomainTestModule : AbpModule
{

}
