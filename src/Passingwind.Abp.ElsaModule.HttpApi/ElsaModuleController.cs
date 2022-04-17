using Passingwind.Abp.ElsaModule.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Passingwind.Abp.ElsaModule;

public abstract class ElsaModuleController : AbpControllerBase
{
    protected ElsaModuleController()
    {
        LocalizationResource = typeof(ElsaModuleResource);
    }
}
