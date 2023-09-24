using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.GlobalCodes;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class GlobalCodeRepository : EfCoreRepository<ElsaModuleDbContext, GlobalCode, Guid>, IGlobalCodeRepository
{
    public GlobalCodeRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task<GlobalCode> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset.FirstOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }

    public virtual async Task<GlobalCode> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        var entity = await dbset.FirstOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);

        return entity ?? throw new EntityNotFoundException(typeof(GlobalCode));
    }

    public virtual async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<GlobalCode>> GetListAsync(string filter = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<GlobalCode>> GetPagedListAsync(int skipCount, int maxResultCount, string filter = null, string sorting = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .OrderBy(sorting ?? nameof(GlobalCode.CreationTime) + " desc")
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> IsNameExistsAsync(string name, Guid[] excludeIds = null, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .WhereIf(excludeIds?.Any() == true, x => !excludeIds.Contains(x.Id))
            .AnyAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }
}
