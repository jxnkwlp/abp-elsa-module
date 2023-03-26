using Passingwind.Abp.ElsaModule;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace Passingwind.WorkflowApp;

[DependsOn(
    typeof(WorkflowAppDomainModule),
    typeof(WorkflowAppApplicationContractsModule),
    typeof(ElsaModuleApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class WorkflowAppApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WorkflowAppApplicationModule>();
        });
    }
}
