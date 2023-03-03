using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.WorkflowInstances;
using Passingwind.Abp.ElsaModule.Common;
using WorkflowInstanceModel = Elsa.Models.WorkflowInstance;

namespace Passingwind.Abp.ElsaModule.Stores;

public class WorkflowInstanceStore : Store<WorkflowInstanceModel, WorkflowInstance, Guid>, IWorkflowInstanceStore
{
    protected override Task<WorkflowInstance> MapToEntityAsync(WorkflowInstanceModel model)
    {
        return Task.FromResult(StoreMapper.MapToEntity(model));
    }

    protected override Task<WorkflowInstance> MapToEntityAsync(WorkflowInstanceModel model, WorkflowInstance entity)
    {
        return Task.FromResult(StoreMapper.MapToEntity(model, entity));
    }

    protected override Task<WorkflowInstanceModel> MapToModelAsync(WorkflowInstance entity)
    {
        return Task.FromResult(StoreMapper.MapToModel(entity));
    }

    public async Task<IEnumerable<TOut>> FindManyAsync<TOut>(ISpecification<WorkflowInstanceModel> specification, Expression<Func<WorkflowInstanceModel, TOut>> funcMapping, IOrderBy<WorkflowInstanceModel> orderBy = null, IPaging paging = null, CancellationToken cancellationToken = default) where TOut : class
    {
        var filter = await MapSpecificationAsync(specification);

        var query = await Repository.WithDetailsAsync();

        query = query.Where(filter);

        if (orderBy != null)
        {
            var orderByExp = orderBy.OrderByExpression.ConvertType<WorkflowInstanceModel, WorkflowInstance>();
            query = (orderBy.SortDirection == SortDirection.Ascending) ? query.OrderBy(orderByExp) : query.OrderByDescending(orderByExp);
        }
        else
        {
            query = query.OrderByDescending(x => x.Id);
        }

        if (paging != null)
            query = query.OrderByDescending(x => x.Id).Skip(paging.Skip).Take(paging.Take);

        var mappingExp = funcMapping.ConvertType<WorkflowInstanceModel, WorkflowInstance, TOut>();

        var selectQuery = query.Select(mappingExp);

        var list = await AsyncExecuter.ToListAsync(selectQuery, cancellationToken);

        return list;
    }

    protected override async Task<Expression<Func<WorkflowInstance, bool>>> MapSpecificationAsync(ISpecification<WorkflowInstanceModel> specification)
    {
        switch (specification)
        {
            case CorrelationIdsSpecification correlationIdsSpecification:
                {
                    return (x) => correlationIdsSpecification.CorrelationIds.Contains(x.CorrelationId) && x.Id == Guid.Parse(correlationIdsSpecification.WorkflowDefinitionId);
                }

            case ManyWorkflowInstanceIdsSpecification manyWorkflowInstanceIdsSpecification:
                {
                    var ids = manyWorkflowInstanceIdsSpecification.Ids.ToList().ConvertAll(Guid.Parse);
                    return (x) => ids.Contains(x.Id);
                }

            case UnfinishedWorkflowSpecification unfinishedWorkflowSpecification:
                {
                    return x => x.WorkflowStatus == WorkflowInstanceStatus.Idle || x.WorkflowStatus == WorkflowInstanceStatus.Running || x.WorkflowStatus == WorkflowInstanceStatus.Suspended;
                }

            case WorkflowCreatedBeforeSpecification workflowCreatedBeforeSpecification:
                {
                    var dateTime = workflowCreatedBeforeSpecification.Instant.ToDateTimeUtc();
                    return x => x.CreationTime <= dateTime;
                }

            case WorkflowDefinitionIdSpecification workflowDefinitionIdSpecification:
                {
                    return (x) => x.WorkflowDefinitionId == Guid.Parse(workflowDefinitionIdSpecification.WorkflowDefinitionId);
                }

            case WorkflowDefinitionVersionIdsSpecification workflowDefinitionVersionIdsSpecification:
                {
                    var ids = workflowDefinitionVersionIdsSpecification.WorkflowDefinitionVersionIds.ToList().ConvertAll(Guid.Parse);
                    return (x) => ids.Contains(x.WorkflowDefinitionVersionId);
                }

            case WorkflowFinishedStatusSpecification workflowFinishedStatusSpecification:
                {
                    return x => x.WorkflowStatus == WorkflowInstanceStatus.Cancelled || x.WorkflowStatus == WorkflowInstanceStatus.Faulted || x.WorkflowStatus == WorkflowInstanceStatus.Finished;
                }

            case WorkflowInstanceContextIdMatchSpecification workflowInstanceContextIdMatchSpecification:
                {
                    return x => x.ContextType!.Contains(workflowInstanceContextIdMatchSpecification.ContextType) && x.ContextId!.Contains(workflowInstanceContextIdMatchSpecification.ContextId);
                }

            case WorkflowInstanceContextMatchSpecification workflowInstanceContextMatchSpecification:
                {
                    return x => x.ContextType!.Contains(workflowInstanceContextMatchSpecification.ContextType);
                }

            case WorkflowInstanceCorrelationIdSpecification workflowInstanceCorrelationIdSpecification:
                {
                    return x => x.WorkflowDefinitionId == Guid.Parse(workflowInstanceCorrelationIdSpecification.DefinitionId) && x.CorrelationId == workflowInstanceCorrelationIdSpecification.CorrelationId;
                }

            case WorkflowInstanceIdSpecification workflowInstanceIdSpecification:
                {
                    return x => x.Id == Guid.Parse(workflowInstanceIdSpecification.Id);
                }

            case WorkflowInstanceIdsSpecification workflowInstanceIdsSpecification:
                {
                    var ids = workflowInstanceIdsSpecification.WorkflowInstanceIds.ToList().ConvertAll(Guid.Parse);
                    return (x) => ids.Contains(x.Id);
                }

            case WorkflowInstanceNameMatchSpecification workflowInstanceNameMatchSpecification:
                {
                    return x => x.Name!.Contains(workflowInstanceNameMatchSpecification.Name);
                }

            case WorkflowIsAlreadyExecutingSpecification workflowIsAlreadyExecutingSpecification:
                {
                    return x => x.WorkflowStatus == WorkflowInstanceStatus.Running || x.WorkflowStatus == WorkflowInstanceStatus.Suspended || x.WorkflowStatus == WorkflowInstanceStatus.Idle;
                }

            case WorkflowSearchTermSpecification workflowSearchTermSpecification:
                {
                    return x => x.Name!.Contains(workflowSearchTermSpecification.SearchTerm) || x.ContextId!.Contains(workflowSearchTermSpecification.SearchTerm);
                }

            case WorkflowStatusSpecification workflowStatusSpecification:
                {
                    var status = (int)workflowStatusSpecification.WorkflowStatus;
                    return x => (int)x.WorkflowStatus == status;
                }

            case WorkflowUnfinishedStatusSpecification workflowUnfinishedStatusSpecification:
                {
                    return x => x.WorkflowStatus == WorkflowInstanceStatus.Idle || x.WorkflowStatus == WorkflowInstanceStatus.Running || x.WorkflowStatus == WorkflowInstanceStatus.Suspended;
                }

            case CorrelationIdSpecification<WorkflowInstanceModel> correlationIdSpecification:
                {
                    return x => x.CorrelationId == correlationIdSpecification.CorrelationId;
                }

            case EntityIdSpecification<WorkflowInstanceModel> entityIdSpecification:
                {
                    return x => x.Id == Guid.Parse(entityIdSpecification.Id);
                }

            case TenantSpecification<WorkflowInstanceModel> tenantSpecification:
                {
                    var tenantId = tenantSpecification.TenantId.ToGuid();
                    return x => x.TenantId == tenantId;
                }

            case AndSpecification<WorkflowInstanceModel> andSpecification:
                {
                    var left = andSpecification.Left;
                    var right = andSpecification.Right;

                    return (await MapSpecificationAsync(left)).And(await MapSpecificationAsync(right));
                }

            default:
                return await base.MapSpecificationAsync(specification);
        }
    }

    protected override Guid ConvertKey(string value)
    {
        return Guid.Parse(value);
    }

}
