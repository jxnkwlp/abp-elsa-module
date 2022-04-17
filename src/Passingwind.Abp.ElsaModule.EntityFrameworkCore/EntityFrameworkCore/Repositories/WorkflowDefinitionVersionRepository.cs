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

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories
{
    public class WorkflowDefinitionVersionRepository : EfCoreRepository<ElsaModuleDbContext, WorkflowDefinitionVersion, Guid>, IWorkflowDefinitionVersionRepository
    {
        public WorkflowDefinitionVersionRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
        {
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
                .IncludeIf(includeDetails, x => x.Definition)
                .OrderByDescending(x => x.Version)
                .Where(x => x.DefinitionId == id)
                .FirstOrDefaultAsync();

            if (entity == null)
                throw new EntityNotFoundException();

            return entity;
        }

        //public async Task<List<WorkflowDefinitionVersion>> GetListAsync(Expression<Func<WorkflowDefinitionVersion, bool>> expression, bool includeDetails = false, CancellationToken cancellationToken = default)
        //{
        //    var dbset = await GetDbSetAsync();
        //    return await dbset
        //        // TODO
        //        //.IncludeIf(includeDetails, TODO )
        //        .WhereIf(expression != null, expression)
        //        .ToListAsync(cancellationToken);
        //}

        public async Task<List<WorkflowDefinitionVersion>> GetPagedListAsync(int skipCount, int maxResultCount, Expression<Func<WorkflowDefinitionVersion, bool>> expression, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();
            return await dbset
                // TODO
                //.IncludeIf(includeDetails, TODO )
                .AsQueryable()
                .WhereIf(expression != null, expression)
                .OrderBy(sorting ?? nameof(WorkflowDefinitionVersion.CreationTime) + " desc")
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        }

    }
}
