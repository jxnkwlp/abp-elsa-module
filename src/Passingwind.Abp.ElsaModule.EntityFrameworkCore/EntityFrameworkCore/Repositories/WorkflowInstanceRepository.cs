using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories
{
    public class WorkflowInstanceRepository : EfCoreRepository<ElsaModuleDbContext, WorkflowInstance, Guid>, IWorkflowInstanceRepository
    {
        public WorkflowInstanceRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(string name = null, Guid? definitionId = null, Guid? definitionVersionId = null, int? version = null, Elsa.Models.WorkflowStatus? status = null, string correlationId = null, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();
            return await dbset
                .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
                .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
                .WhereIf(version.HasValue, x => x.Version == version)
                .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
                .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
                .WhereIf(!string.IsNullOrEmpty(name), x => EF.Functions.Like(x.Name, $"%{name}%"))
                .LongCountAsync(cancellationToken);
        }

        public async Task<List<WorkflowInstance>> GetListAsync(string name = null, Guid? definitionId = null, Guid? definitionVersionId = null, int? version = null, Elsa.Models.WorkflowStatus? status = null, string correlationId = null, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();
            return await dbset
                .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
                .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
                .WhereIf(version.HasValue, x => x.Version == version)
                .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
                .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
                .WhereIf(!string.IsNullOrEmpty(name), x => EF.Functions.Like(x.Name, $"%{name}%"))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<WorkflowInstance>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, string name = null, Guid? definitionId = null, Guid? definitionVersionId = null, int? version = null, Elsa.Models.WorkflowStatus? status = null, string correlationId = null, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();
            return await dbset
                .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
                .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
                .WhereIf(version.HasValue, x => x.Version == version)
                .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
                .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
                .WhereIf(!string.IsNullOrEmpty(name), x => EF.Functions.Like(x.Name, $"%{name}%"))
                .OrderBy(sorting ?? nameof(WorkflowInstance.CreationTime) + " desc")
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        }

    }
}
