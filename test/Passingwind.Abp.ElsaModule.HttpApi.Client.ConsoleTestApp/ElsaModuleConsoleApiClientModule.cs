using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ElsaModuleHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class ElsaModuleConsoleApiClientModule : AbpModule
{

}
