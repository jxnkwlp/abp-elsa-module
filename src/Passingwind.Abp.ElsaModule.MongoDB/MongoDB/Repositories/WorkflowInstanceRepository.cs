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

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories
{
    public class WorkflowInstanceRepository : MongoDbRepository<IElsaModuleMongoDbContext, WorkflowInstance, Guid>, IWorkflowInstanceRepository
    {
        public WorkflowInstanceRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(string name = null, Guid? definitionId = null, Guid? definitionVersionId = null, int? version = null, WorkflowInstanceStatus? status = null, string correlationId = null, CancellationToken cancellationToken = default)
        {
            var query = await GetMongoQueryableAsync(cancellationToken);
            return await query
                .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
                .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
                .WhereIf(version.HasValue, x => x.Version == version)
                .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
                .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
                .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name))
                .As<IMongoQueryable<WorkflowInstance>>()
                .LongCountAsync(cancellationToken);
        }

        public async Task<List<WorkflowInstance>> GetListAsync(string name = null, Guid? definitionId = null, Guid? definitionVersionId = null, int? version = null, WorkflowInstanceStatus? status = null, string correlationId = null, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            var query = await GetMongoQueryableAsync(cancellationToken);
            return await query
                .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
                .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
                .WhereIf(version.HasValue, x => x.Version == version)
                .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
                .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
                .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name))
                .As<IMongoQueryable<WorkflowInstance>>()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<WorkflowInstance>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, string name = null, Guid? definitionId = null, Guid? definitionVersionId = null, int? version = null, WorkflowInstanceStatus? status = null, string correlationId = null, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            var query = await GetMongoQueryableAsync(cancellationToken);
            return await query
                .WhereIf(definitionId.HasValue, x => x.WorkflowDefinitionId == definitionId)
                .WhereIf(definitionVersionId.HasValue, x => x.WorkflowDefinitionVersionId == definitionVersionId)
                .WhereIf(version.HasValue, x => x.Version == version)
                .WhereIf(status.HasValue, x => x.WorkflowStatus == status)
                .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
                .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name))
                .OrderBy(sorting ?? nameof(WorkflowInstance.CreationTime) + " desc")
                .As<IMongoQueryable<WorkflowInstance>>()
                .PageBy<WorkflowInstance, IMongoQueryable<WorkflowInstance>>(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<DateTime, int>> GetStatusDateCountStatisticsAsync(WorkflowInstanceStatus workflowStatus, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var query = await GetMongoQueryableAsync(cancellationToken);

            var list = await query
                       .Where(x => x.WorkflowStatus == workflowStatus && x.CreationTime.Date >= startDate.Date && x.CreationTime.Date <= endDate.Date)
                       .Select(x => new { x.Id, x.CreationTime })
                       .ToListAsync(cancellationToken);

            return list
                    .GroupBy(x => x.CreationTime.Date)
                    .ToDictionary(x => x.Key.Date, x => x.Count());
        }

    }
}
