using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passingwind.Abp.ElsaModule.Groups;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class WorkflowGroupRepository : MongoDbRepository<ElsaModuleMongoDbContext, WorkflowGroup, Guid>, IWorkflowGroupRepository
{
    public WorkflowGroupRepository(IMongoDbContextProvider<ElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .As<IMongoQueryable<WorkflowGroup>>()
            .LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<WorkflowGroup>> GetListAsync(string filter = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .As<IMongoQueryable<WorkflowGroup>>()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<WorkflowGroup>> GetPagedListAsync(int skipCount, int maxResultCount, string filter = null, string sorting = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync();

        return await query
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .OrderBy(sorting ?? nameof(WorkflowGroup.CreationTime) + " desc")
            .PageBy(skipCount, maxResultCount)
            .As<IMongoQueryable<WorkflowGroup>>()
            .ToListAsync(cancellationToken);
    }
}
