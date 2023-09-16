using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(ElsaModuleApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class ElsaModuleHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(ElsaModuleApplicationContractsModule).Assembly,
            ElsaModuleRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<ElsaModuleHttpApiClientModule>());
    }
}
