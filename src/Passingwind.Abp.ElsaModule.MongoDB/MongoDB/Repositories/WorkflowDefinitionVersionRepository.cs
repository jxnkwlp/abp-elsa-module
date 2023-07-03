using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class WorkflowDefinitionVersionRepository : MongoDbRepository<IElsaModuleMongoDbContext, WorkflowDefinitionVersion, Guid>, IWorkflowDefinitionVersionRepository
{
    public WorkflowDefinitionVersionRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<WorkflowDefinitionVersion> FindByVersionAsync(Guid definitionId, int version, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        var entity = await query
            .Where(x => x.DefinitionId == definitionId && x.Version == version)
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
    }

    public async Task<WorkflowDefinitionVersion> GetByVersionAsync(Guid definitionId, int version, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        var entity = await query
            .Where(x => x.DefinitionId == definitionId && x.Version == version)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException();

        return entity;
    }

    public async Task<long> GetCountAsync(Expression<Func<WorkflowDefinitionVersion, bool>> expression, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);
        return await query
            .WhereIf(expression != null, expression)
            .LongCountAsync(cancellationToken);
    }

    public async Task<WorkflowDefinitionVersion> GetLatestAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        var entity = await query
            .OrderByDescending(x => x.Version)
            .Where(x => x.DefinitionId == id)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException();

        return entity;
    }

    public async Task<List<WorkflowDefinitionVersion>> GetListAsync(IEnumerable<Guid> definitionIds = null, bool? isLatest = null, bool? isPublished = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return await query
            .WhereIf(definitionIds != null, x => definitionIds.Contains(x.DefinitionId))
            .WhereIf(isLatest.HasValue, x => x.IsLatest == isLatest)
            .WhereIf(isPublished.HasValue, x => x.IsPublished == isPublished)
            .As<IMongoQueryable<WorkflowDefinitionVersion>>()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkflowDefinitionVersion>> GetPagedListAsync(int skipCount, int maxResultCount, Expression<Func<WorkflowDefinitionVersion, bool>> expression, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return await query
            .WhereIf(expression != null, expression)
            .OrderBy(sorting ?? nameof(WorkflowDefinitionVersion.CreationTime) + " desc")
            .As<IMongoQueryable<WorkflowDefinitionVersion>>()
            .PageBy<WorkflowDefinitionVersion, IMongoQueryable<WorkflowDefinitionVersion>>(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<int?> GetPreviousVersionNumberAsync(Guid definitionId, int version, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        return await query
            .Where(x => x.DefinitionId == definitionId)
            .OrderByDescending(x => x.Version)
            .Where(x => x.Version < version)
            .Select(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
