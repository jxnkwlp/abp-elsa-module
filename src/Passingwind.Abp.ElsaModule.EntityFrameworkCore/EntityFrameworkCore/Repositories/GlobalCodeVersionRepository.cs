using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.GlobalCodes;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class GlobalCodeVersionRepository : EfCoreRepository<ElsaModuleDbContext, GlobalCodeVersion, Guid>, IGlobalCodeVersionRepository
{
    public GlobalCodeVersionRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<GlobalCodeVersion>> GetListAsync(Guid codeId, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .Where(x => x.GlobalCodeId == codeId)
            .ToListAsync(cancellationToken);
    }
}
