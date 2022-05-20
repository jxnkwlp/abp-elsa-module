using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.WorkflowInstances;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.Timing;
using WorkflowInstanceModel = Elsa.Models.WorkflowInstance;
using WorkflowStatus = Elsa.Models.WorkflowStatus;

namespace Passingwind.Abp.ElsaModule.Stores
{
    public class WorkflowInstanceStore : Store<WorkflowInstanceModel, WorkflowInstance, Guid>, IWorkflowInstanceStore
    {
        private readonly IStoreMapper _storeMapper;
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IClock _clock;

        public WorkflowInstanceStore(ILoggerFactory loggerFactory, IRepository<WorkflowInstance, Guid> repository, IAsyncQueryableExecuter asyncQueryableExecuter, IStoreMapper storeMapper, IWorkflowDefinitionRepository workflowDefinitionRepository, IClock clock) : base(loggerFactory, repository, asyncQueryableExecuter)
        {
            _storeMapper = storeMapper;
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _clock = clock;
        }

        protected override async Task<WorkflowInstance> MapToEntityAsync(WorkflowInstanceModel model)
        {
            var entity = _storeMapper.MapToEntity(model);

            var workflowDefinition = await _workflowDefinitionRepository.GetAsync(entity.DefinitionId);

            entity.Name = $"{workflowDefinition.Name}-{_clock.Now.ToString("yyyyMMddHHmmss")}";

            return entity;
        }

        protected override Task<WorkflowInstance> MapToEntityAsync(WorkflowInstanceModel model, WorkflowInstance entity)
        {
            return Task.FromResult(_storeMapper.MapToEntity(model, entity));
        }

        protected override Task<WorkflowInstanceModel> MapToModelAsync(WorkflowInstance entity)
        {
            return Task.FromResult(_storeMapper.MapToModel(entity));
        }

        protected override async Task<Expression<Func<WorkflowInstance, bool>>> MapSpecificationAsync(ISpecification<WorkflowInstanceModel> specification)
        {
            if (specification is CorrelationIdsSpecification correlationIdsSpecification)
            {
                return (x) => correlationIdsSpecification.CorrelationIds.Contains(x.CorrelationId) && x.Id == Guid.Parse(correlationIdsSpecification.WorkflowDefinitionId);
            }
            else if (specification is ManyWorkflowInstanceIdsSpecification manyWorkflowInstanceIdsSpecification)
            {
                var ids = manyWorkflowInstanceIdsSpecification.Ids.ToList().ConvertAll(Guid.Parse);
                return (x) => ids.Contains(x.Id);
            }
            else if (specification is UnfinishedWorkflowSpecification unfinishedWorkflowSpecification)
            {
                return x => x.WorkflowStatus == WorkflowStatus.Idle || x.WorkflowStatus == WorkflowStatus.Running || x.WorkflowStatus == WorkflowStatus.Suspended;
            }
            else if (specification is WorkflowCreatedBeforeSpecification workflowCreatedBeforeSpecification)
            {
                var dateTime = workflowCreatedBeforeSpecification.Instant.ToDateTimeUtc();
                return x => x.CreationTime <= dateTime;
            }
            else if (specification is WorkflowDefinitionIdSpecification workflowDefinitionIdSpecification)
            {
                return (x) => x.DefinitionId == Guid.Parse(workflowDefinitionIdSpecification.WorkflowDefinitionId);
            }
            else if (specification is WorkflowDefinitionVersionIdsSpecification workflowDefinitionVersionIdsSpecification)
            {
                var ids = workflowDefinitionVersionIdsSpecification.WorkflowDefinitionVersionIds.ToList().ConvertAll(Guid.Parse);
                return (x) => ids.Contains(x.DefinitionVersionId);
            }
            else if (specification is WorkflowFinishedStatusSpecification workflowFinishedStatusSpecification)
            {
                return x => x.WorkflowStatus == WorkflowStatus.Cancelled || x.WorkflowStatus == WorkflowStatus.Faulted || x.WorkflowStatus == WorkflowStatus.Finished;
            }
            else if (specification is WorkflowInstanceContextIdMatchSpecification workflowInstanceContextIdMatchSpecification)
            {
                return x => x.ContextType!.Contains(workflowInstanceContextIdMatchSpecification.ContextType) && x.ContextId!.Contains(workflowInstanceContextIdMatchSpecification.ContextId);
            }
            else if (specification is WorkflowInstanceContextMatchSpecification workflowInstanceContextMatchSpecification)
            {
                return x => x.ContextType!.Contains(workflowInstanceContextMatchSpecification.ContextType);
            }
            else if (specification is WorkflowInstanceCorrelationIdSpecification workflowInstanceCorrelationIdSpecification)
            {
                return x => x.DefinitionId == Guid.Parse(workflowInstanceCorrelationIdSpecification.DefinitionId) && x.CorrelationId == workflowInstanceCorrelationIdSpecification.CorrelationId;
            }
            else if (specification is WorkflowInstanceIdSpecification workflowInstanceIdSpecification)
            {
                return x => x.Id == Guid.Parse(workflowInstanceIdSpecification.Id);
            }
            else if (specification is WorkflowInstanceIdsSpecification workflowInstanceIdsSpecification)
            {
                var ids = workflowInstanceIdsSpecification.WorkflowInstanceIds.ToList().ConvertAll(Guid.Parse);
                return (x) => ids.Contains(x.Id);
            }
            else if (specification is WorkflowInstanceNameMatchSpecification workflowInstanceNameMatchSpecification)
            {
                return x => x.Name!.Contains(workflowInstanceNameMatchSpecification.Name);
            }
            else if (specification is WorkflowIsAlreadyExecutingSpecification workflowIsAlreadyExecutingSpecification)
            {
                return x => x.WorkflowStatus == WorkflowStatus.Running || x.WorkflowStatus == WorkflowStatus.Suspended || x.WorkflowStatus == WorkflowStatus.Idle;
            }
            else if (specification is WorkflowSearchTermSpecification workflowSearchTermSpecification)
            {
                return x => x.Name!.Contains(workflowSearchTermSpecification.SearchTerm) || x.ContextId!.Contains(workflowSearchTermSpecification.SearchTerm);
            }
            else if (specification is WorkflowStatusSpecification workflowStatusSpecification)
            {
                return x => x.WorkflowStatus == workflowStatusSpecification.WorkflowStatus;
            }
            else if (specification is WorkflowUnfinishedStatusSpecification workflowUnfinishedStatusSpecification)
            {
                return x => x.WorkflowStatus == WorkflowStatus.Idle || x.WorkflowStatus == WorkflowStatus.Running || x.WorkflowStatus == WorkflowStatus.Suspended;
            }
            else if (specification is CorrelationIdSpecification<WorkflowInstanceModel> correlationIdSpecification)
            {
                return x => x.CorrelationId == correlationIdSpecification.CorrelationId;
            }
            else if (specification is EntityIdSpecification<WorkflowInstanceModel> entityIdSpecification)
            {
                return x => x.Id == Guid.Parse(entityIdSpecification.Id);
            }
            else if (specification is TenantSpecification<WorkflowInstanceModel> tenantSpecification)
            {
                var tenantId = tenantSpecification.TenantId.ToGuid();
                return x => x.TenantId == tenantId;
            }
            else if (specification is AndSpecification<WorkflowInstanceModel> andSpecification)
            {
                var left = andSpecification.Left;
                var right = andSpecification.Right;

                return (await MapSpecificationAsync(left)).And(await MapSpecificationAsync(right));
            }
            else
                return await base.MapSpecificationAsync(specification);
        }

        protected override Guid ConvertKey(string value)
        {
            return Guid.Parse(value);
        }
    }
}
