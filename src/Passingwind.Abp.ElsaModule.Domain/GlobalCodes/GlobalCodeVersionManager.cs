using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public class GlobalCodeVersionManager : DomainService
{
    private readonly IGlobalCodeVersionRepository _globalCodeVersionRepository;

    public GlobalCodeVersionManager(IGlobalCodeVersionRepository globalCodeVersionRepository)
    {
        _globalCodeVersionRepository = globalCodeVersionRepository;
    }
}
