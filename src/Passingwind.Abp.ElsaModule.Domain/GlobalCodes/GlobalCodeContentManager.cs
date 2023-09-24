using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public class GlobalCodeContentManager : DomainService
{
    private readonly IGlobalCodeContentRepository _globalCodeContentRepository;

    public GlobalCodeContentManager(IGlobalCodeContentRepository globalCodeContentRepository)
    {
        _globalCodeContentRepository = globalCodeContentRepository;
    }
}
