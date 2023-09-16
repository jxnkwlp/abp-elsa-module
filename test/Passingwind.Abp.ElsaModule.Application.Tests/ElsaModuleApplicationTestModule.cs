using Volo.Abp.Modularity;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(ElsaModuleApplicationModule),
    typeof(ElsaModuleDomainTestModule)
    )]
public class ElsaModuleApplicationTestModule : AbpModule
{
}
