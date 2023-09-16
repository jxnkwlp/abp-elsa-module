using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class WorkflowDefinitionVersionRepository : EfCoreRepository<ElsaModuleDbContext, WorkflowDefinitionVersion, Guid>, IWorkflowDefinitionVersionRepository
{
    public WorkflowDefinitionVersionRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<WorkflowDefinitionVersion> FindByVersionAsync(Guid definitionId, int version, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .IncludeIf(includeDetails, x => x.Activities)
            .IncludeIf(includeDetails, x => x.Connections)
            .Where(x => x.DefinitionId == definitionId && x.Version == version)
            .FirstOrDefaultAsync();
    }

    public async Task<WorkflowDefinitionVersion> GetByVersionAsync(Guid definitionId, int version, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        var entity = await dbset
            .IncludeIf(includeDetails, x => x.Activities)
            .IncludeIf(includeDetails, x => x.Connections)
            .Where(x => x.DefinitionId == definitionId && x.Version == version)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException();

        return entity;
    }

    public async Task<long> GetCountAsync(Expression<Func<WorkflowDefinitionVersion, bool>> expression, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            .AsNoTracking()
            .AsQueryable()
            .WhereIf(expression != null, expression)
            .LongCountAsync(cancellationToken);
    }

    public async Task<WorkflowDefinitionVersion> GetLatestAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        var entity = await dbset
            .IncludeIf(includeDetails, x => x.Activities)
            .IncludeIf(includeDetails, x => x.Connections)
            .OrderByDescending(x => x.Version)
            .Where(x => x.DefinitionId == id)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException();

        return entity;
    }

    public async Task<List<WorkflowDefinitionVersion>> GetListAsync(IEnumerable<Guid> definitionIds = null, bool? isLatest = null, bool? isPublished = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .WhereIf(definitionIds != null, x => definitionIds.Contains(x.DefinitionId))
            .WhereIf(isLatest.HasValue, x => x.IsLatest == isLatest)
            .WhereIf(isPublished.HasValue, x => x.IsPublished == isPublished)
            .IncludeIf(includeDetails, x => x.Activities)
            .IncludeIf(includeDetails, x => x.Connections)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkflowDefinitionVersion>> GetPagedListAsync(int skipCount, int maxResultCount, Expression<Func<WorkflowDefinitionVersion, bool>> expression, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            .IncludeIf(includeDetails, x => x.Activities)
            .IncludeIf(includeDetails, x => x.Connections)
            .AsQueryable()
            .WhereIf(expression != null, expression)
            .OrderBy(sorting ?? nameof(WorkflowDefinitionVersion.CreationTime) + " desc")
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<int?> GetPreviousVersionNumberAsync(Guid definitionId, int version, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            .Where(x => x.DefinitionId == definitionId)
            .OrderByDescending(x => x.Version)
            .Where(x => x.Version < version)
            .Select(x => x.Version)
            .FirstOrDefaultAsync();
    }
}
