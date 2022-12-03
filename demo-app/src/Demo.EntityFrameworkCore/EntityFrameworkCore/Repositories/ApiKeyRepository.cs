using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Demo.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Demo.EntityFrameworkCore.Repositories;

public class ApiKeyRepository : EfCoreRepository<DemoDbContext, ApiKey, Guid>, IApiKeyRepository
{
    public ApiKeyRepository(IDbContextProvider<DemoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> GetCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            .Where(x => x.UserId == userId)
            .LongCountAsync(cancellationToken);
    }

    public async Task<List<ApiKey>> GetListAsync(Guid userId, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ApiKey>> GetPagedListAsync(int skipCount, int maxResultCount, Guid userId, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            .Where(x => x.UserId == userId)
            .OrderBy(sorting ?? nameof(ApiKey.CreationTime) + " desc")
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

}
