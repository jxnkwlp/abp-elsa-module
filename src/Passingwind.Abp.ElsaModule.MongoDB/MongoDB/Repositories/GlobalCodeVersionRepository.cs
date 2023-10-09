using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passingwind.Abp.ElsaModule.GlobalCodes;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class GlobalCodeVersionRepository : MongoDbRepository<ElsaModuleMongoDbContext, GlobalCodeVersion, Guid>, IGlobalCodeVersionRepository
{
    public GlobalCodeVersionRepository(IMongoDbContextProvider<ElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<GlobalCodeVersion>> GetListAsync(Guid codeId, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query
            .Where(x => x.GlobalCodeId == codeId)
            .As<IMongoQueryable<GlobalCodeVersion>>()
            .ToListAsync(cancellationToken);
    }
}
