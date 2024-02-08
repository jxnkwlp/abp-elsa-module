using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public class GlobalCodeContentManager : DomainService
{
    private readonly IGlobalCodeContentRepository _globalCodeContentRepository;

    public GlobalCodeContentManager(IGlobalCodeContentRepository globalCodeContentRepository)
    {
        _globalCodeContentRepository = globalCodeContentRepository;
    }
}
