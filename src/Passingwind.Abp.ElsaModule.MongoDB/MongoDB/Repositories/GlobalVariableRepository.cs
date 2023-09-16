using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class GlobalVariableRepository : MongoDbRepository<IElsaModuleMongoDbContext, GlobalVariable, Guid>, IGlobalVariableRepository
{
    public GlobalVariableRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<GlobalVariable> FindAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        return await this.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
    }

    public async Task<GlobalVariable> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        var entity = await this.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(typeof(GlobalVariable));

        return entity;
    }

    public async Task<long> CountAsync(string filter, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Key.Contains(filter))
            .As<IMongoQueryable<GlobalVariable>>()
            .LongCountAsync(cancellationToken);
    }

    public async Task<List<GlobalVariable>> GetListAsync(string filter, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
           .WhereIf(!string.IsNullOrEmpty(filter), x => x.Key.Contains(filter))
           .As<IMongoQueryable<GlobalVariable>>()
           .ToListAsync(cancellationToken);
    }

    public async Task<List<GlobalVariable>> GetPagedListAsync(int skipCount, int maxResultCount, string filter, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Key.Contains(filter))
            .As<IMongoQueryable<GlobalVariable>>()
            .PageBy(skipCount, maxResultCount)
            .OrderBy(sorting ?? nameof(GlobalVariable.CreationTime) + " desc")
            .As<IMongoQueryable<GlobalVariable>>()
            .ToListAsync(cancellationToken);
    }
}
