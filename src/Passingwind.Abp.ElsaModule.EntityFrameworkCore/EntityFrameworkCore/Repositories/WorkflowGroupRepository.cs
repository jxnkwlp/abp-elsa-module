using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class WorkflowGroupRepository : EfCoreRepository<ElsaModuleDbContext, WorkflowGroup, Guid>, IWorkflowGroupRepository
{
    public WorkflowGroupRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> GetCountAsync(string filter, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            // TODO
            //.WhereIf(!string.IsNullOrEmpty(filter),  )
            .LongCountAsync(cancellationToken);
    }

    public async Task<List<WorkflowGroup>> GetListAsync(string filter, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            // TODO
            //.IncludeIf(includeDetails, TODO )
            //.WhereIf(!string.IsNullOrEmpty(filter), TODO )
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkflowGroup>> GetPagedListAsync(int skipCount, int maxResultCount, string filter, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();
        return await dbset
            // TODO
            //.IncludeIf(includeDetails, TODO )
            //.WhereIf(!string.IsNullOrEmpty(filter), TODO )
            .PageBy(skipCount, maxResultCount)
            .OrderBy(sorting ?? nameof(WorkflowGroup.CreationTime) + " desc")
            .ToListAsync(cancellationToken);
    }
}
