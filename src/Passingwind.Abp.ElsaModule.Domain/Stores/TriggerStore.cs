using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.Triggers;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using TriggerModel = Elsa.Models.Trigger;

namespace Passingwind.Abp.ElsaModule.Stores
{
    public class TriggerStore : Store<TriggerModel, Trigger, Guid>, ITriggerStore
    {
        private readonly IStoreMapper _storeMapper;

        public TriggerStore(ILoggerFactory loggerFactory, IRepository<Trigger, Guid> repository, IAsyncQueryableExecuter asyncQueryableExecuter, IStoreMapper storeMapper) : base(loggerFactory, repository, asyncQueryableExecuter)
        {
            _storeMapper = storeMapper;
        }

        protected override Task<Trigger> MapToEntityAsync(TriggerModel model)
        {
            return Task.FromResult(_storeMapper.MapToEntity(model));
        }

        protected override Task<Trigger> MapToEntityAsync(TriggerModel model, Trigger entity)
        {
            return Task.FromResult(_storeMapper.MapToEntity(model, entity));
        }

        protected override Task<TriggerModel> MapToModelAsync(Trigger entity)
        {
            return Task.FromResult(_storeMapper.MapToModel(entity));
        }

        protected override async Task<Expression<Func<Trigger, bool>>> MapSpecificationAsync(ISpecification<TriggerModel> specification)
        {
            if (specification is WorkflowDefinitionIdSpecification idSpecification)
            {
                var id = idSpecification.WorkflowDefinitionId.ToGuid();
                return (x) => x.WorkflowDefinitionId == id;
            }
            else if (specification is WorkflowDefinitionIdsSpecification idsSpecification)
            {
                var ids = idsSpecification.WorkflowDefinitionIds.ToList().ConvertAll(Guid.Parse);
                return (x) => ids.Contains(x.WorkflowDefinitionId);
            }
            else if (specification is TriggerSpecification triggerSpecification)
            {
                var tenantId = triggerSpecification.TenantId.ToGuid();
                return trigger => trigger.TenantId == tenantId && trigger.ActivityType == triggerSpecification.ActivityType && triggerSpecification.Hashes.Contains(trigger.Hash);
            }
            else if (specification is TriggerModelTypeSpecification modelTypeSpecification)
            {
                var tenantId = modelTypeSpecification.TenantId.ToGuid();
                return x => modelTypeSpecification.ModelType.Equals(x.ModelType) && x.TenantId == tenantId;
            }
            else if (specification is TriggerIdsSpecification triggerIdsSpecification)
            {
                var ids = triggerIdsSpecification.Ids.ToList().ConvertAll(Guid.Parse);
                return x => ids.Contains(x.Id);
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
