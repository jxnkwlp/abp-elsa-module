using Passingwind.Abp.ElsaModule.Localization;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule;

public abstract class ElsaModuleAppService : ApplicationService
{
    protected ElsaModuleAppService()
    {
        LocalizationResource = typeof(ElsaModuleResource);
        ObjectMapperContext = typeof(ElsaModuleApplicationModule);
    }
}
