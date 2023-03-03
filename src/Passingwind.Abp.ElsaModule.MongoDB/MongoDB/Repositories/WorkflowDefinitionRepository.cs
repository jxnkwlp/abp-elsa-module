using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class WorkflowDefinitionRepository : MongoDbRepository<IElsaModuleMongoDbContext, WorkflowDefinition, Guid>, IWorkflowDefinitionRepository
{
    public WorkflowDefinitionRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> CountAsync(string name = null, bool? isSingleton = null, int? publishedVersion = null, string channel = null, string tag = null, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return await query
              .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name) || x.DisplayName.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(channel), x => x.Channel.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(tag), x => x.Channel.Contains(tag))
              .WhereIf(isSingleton.HasValue, x => x.IsSingleton == isSingleton)
              .WhereIf(publishedVersion.HasValue, x => x.PublishedVersion == publishedVersion)
              .As<IMongoQueryable<WorkflowDefinition>>()
              .LongCountAsync(cancellationToken);
    }

    public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken = GetCancellationToken(cancellationToken);
        var q = await GetMongoQueryableAsync(cancellationToken);
        return await q.LongCountAsync(cancellationToken);
    }

    public async Task<Guid> GetIdByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query.Where(x => x.Name == name).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> GetIdByTagAsync(string tags, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return await query.Where(x => x.Tag == tags).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid[]> GetIdsByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return (await query.Where(x => names.Contains(x.Name)).Select(x => x.Id).ToListAsync(cancellationToken)).ToArray();
    }

    public async Task<Guid[]> GetIdsByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return (await query.Where(x => tags.Contains(x.Tag)).Select(x => x.Id).ToListAsync(cancellationToken)).ToArray();
    }

    public async Task<List<WorkflowDefinition>> GetListAsync(string name = null, bool? isSingleton = null, int? publishedVersion = null, string channel = null, string tag = null, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return await query
              .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name) || x.DisplayName.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(channel), x => x.Channel.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(tag), x => x.Channel.Contains(tag))
              .WhereIf(isSingleton.HasValue, x => x.IsSingleton == isSingleton)
              .WhereIf(publishedVersion.HasValue, x => x.PublishedVersion == publishedVersion)
              .As<IMongoQueryable<WorkflowDefinition>>()
              .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkflowDefinition>> GetPagedListAsync(int skipCount, int maxResultCount, string name = null, bool? isSingleton = null, int? publishedVersion = null, string channel = null, string tag = null, string ordering = null, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return await query
              .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name) || x.DisplayName.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(channel), x => x.Channel.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(tag), x => x.Channel.Contains(tag))
              .WhereIf(isSingleton.HasValue, x => x.IsSingleton == isSingleton)
              .WhereIf(publishedVersion.HasValue, x => x.PublishedVersion == publishedVersion)
              .OrderBy<WorkflowDefinition>(ordering ?? $"{nameof(WorkflowDefinition.CreationTime)} desc")
              .PageBy(skipCount, maxResultCount)
              .As<IMongoQueryable<WorkflowDefinition>>()
              .ToListAsync(cancellationToken);
    }

}
