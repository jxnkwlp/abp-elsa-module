using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passingwind.Abp.ElsaModule.GlobalCodes;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class GlobalCodeRepository : MongoDbRepository<ElsaModuleMongoDbContext, GlobalCode, Guid>, IGlobalCodeRepository
{
    public GlobalCodeRepository(IMongoDbContextProvider<ElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task<GlobalCode> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query.FirstOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }

    public virtual async Task<GlobalCode> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        var entity = await query.FirstOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);

        return entity ?? throw new EntityNotFoundException(typeof(GlobalCode));
    }

    public virtual async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .As<IMongoQueryable<GlobalCode>>()
            .LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<GlobalCode>> GetListAsync(string filter = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .As<IMongoQueryable<GlobalCode>>()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<GlobalCode>> GetPagedListAsync(int skipCount, int maxResultCount, string filter = null, string sorting = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .OrderBy(sorting ?? nameof(GlobalCode.CreationTime) + " desc")
            .PageBy(skipCount, maxResultCount)
            .As<IMongoQueryable<GlobalCode>>()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> IsNameExistsAsync(string name, Guid[] excludeIds = null, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query
            .WhereIf(excludeIds?.Any() == true, x => !excludeIds.Contains(x.Id))
            .As<IMongoQueryable<GlobalCode>>()
            .AnyAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }
}
