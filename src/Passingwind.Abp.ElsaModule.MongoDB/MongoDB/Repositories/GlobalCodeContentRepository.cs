using System;
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

public class GlobalCodeContentRepository : MongoDbRepository<ElsaModuleMongoDbContext, GlobalCodeContent, Guid>, IGlobalCodeContentRepository
{
    public GlobalCodeContentRepository(IMongoDbContextProvider<ElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<GlobalCodeContent> FindByVersionAsync(Guid codeId, int version, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query.FirstOrDefaultAsync(x => x.GlobalCodeId == codeId && x.Version == version, cancellationToken: cancellationToken);
    }

    public async Task<bool> IsVersionExistsAsync(Guid codeId, int version, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query.Where(x => x.GlobalCodeId == codeId && x.Version == version).AnyAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateContentAsync(Guid codeId, int version, string content, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        if (await query.AnyAsync(x => x.GlobalCodeId == codeId && x.Version == version, cancellationToken: cancellationToken))
        {
            var entity = await GetAsync(x => x.GlobalCodeId == codeId && x.Version == version, cancellationToken: cancellationToken);
            entity.Content = content;

            await UpdateAsync(entity, cancellationToken: cancellationToken);
        }
        else
        {
            await InsertAsync(new GlobalCodeContent(GuidGenerator.Create(), codeId, version, content, CurrentTenant.Id), cancellationToken: cancellationToken);
        }
    }
}
