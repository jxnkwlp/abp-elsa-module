using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public class GlobalCodeVersionManager : DomainService
{
    private readonly IGlobalCodeVersionRepository _globalCodeVersionRepository;

    public GlobalCodeVersionManager(IGlobalCodeVersionRepository globalCodeVersionRepository)
    {
        _globalCodeVersionRepository = globalCodeVersionRepository;
    }
}
