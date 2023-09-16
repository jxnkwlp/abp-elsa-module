using Passingwind.Abp.ElsaModule.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class ElsaModuleDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<ElsaModuleDomainSharedModule>());

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<ElsaModuleResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/ElsaModule");
        });

        Configure<AbpExceptionLocalizationOptions>(options => options.MapCodeNamespace("ElsaModule", typeof(ElsaModuleResource)));
    }
}
