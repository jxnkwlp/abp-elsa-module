using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.Triggers;
using Passingwind.Abp.ElsaModule.Common;
using TriggerModel = Elsa.Models.Trigger;

namespace Passingwind.Abp.ElsaModule.Stores;

public class TriggerStore : Store<TriggerModel, Trigger, Guid>, ITriggerStore
{
    protected override Task<Trigger> MapToEntityAsync(TriggerModel model)
    {
        return Task.FromResult(StoreMapper.MapToEntity(model));
    }

    protected override Task<Trigger> MapToEntityAsync(TriggerModel model, Trigger entity)
    {
        return Task.FromResult(StoreMapper.MapToEntity(model, entity));
    }

    protected override Task<TriggerModel> MapToModelAsync(Trigger entity)
    {
        return Task.FromResult(StoreMapper.MapToModel(entity));
    }

    protected override async Task<Expression<Func<Trigger, bool>>> MapSpecificationAsync(ISpecification<TriggerModel> specification)
    {
        switch (specification)
        {
            case WorkflowDefinitionIdSpecification idSpecification:
                {
                    var id = idSpecification.WorkflowDefinitionId.ToGuid();
                    return (x) => x.WorkflowDefinitionId == id;
                }

            case WorkflowDefinitionIdsSpecification idsSpecification:
                {
                    var ids = idsSpecification.WorkflowDefinitionIds.ToList().ConvertAll(Guid.Parse);
                    return (x) => ids.Contains(x.WorkflowDefinitionId);
                }

            case TriggerSpecification triggerSpecification:
                {
                    var tenantId = triggerSpecification.TenantId.ToGuid();
                    return trigger => trigger.TenantId == tenantId && trigger.ActivityType == triggerSpecification.ActivityType && triggerSpecification.Hashes.Contains(trigger.Hash);
                }

            case TriggerModelTypeSpecification modelTypeSpecification:
                {
                    var tenantId = modelTypeSpecification.TenantId.ToGuid();
                    return x => modelTypeSpecification.ModelType.Equals(x.ModelType) && x.TenantId == tenantId;
                }

            case TriggerIdsSpecification triggerIdsSpecification:
                {
                    var ids = triggerIdsSpecification.Ids.ToList().ConvertAll(Guid.Parse);
                    return x => ids.Contains(x.Id);
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
