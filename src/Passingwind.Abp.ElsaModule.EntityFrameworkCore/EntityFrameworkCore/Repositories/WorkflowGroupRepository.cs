using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.Groups;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class WorkflowGroupRepository : EfCoreRepository<ElsaModuleDbContext, WorkflowGroup, Guid>, IWorkflowGroupRepository
{
    public WorkflowGroupRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<WorkflowGroup>> GetListAsync(string filter = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<WorkflowGroup>> GetPagedListAsync(int skipCount, int maxResultCount, string filter = null, string sorting = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
            .WhereIf(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .OrderBy(sorting ?? nameof(WorkflowGroup.CreationTime) + " desc")
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
