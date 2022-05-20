using System.Threading.Tasks;
using Elsa.Events;
using MediatR;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Passingwind.Abp.ElsaModule.Stores
{
    public class EntityEventHandler :
        ILocalEventHandler<EntityCreatedEventData<WorkflowDefinitionVersion>>,
        ILocalEventHandler<EntityUpdatedEventData<WorkflowDefinitionVersion>>,
        ILocalEventHandler<EntityDeletedEventData<WorkflowDefinitionVersion>>,
        ILocalEventHandler<EntityUpdatedEventData<WorkflowDefinition>>,
        ILocalEventHandler<EntityDeletedEventData<WorkflowInstance>>,
        ITransientDependency
    {
        private readonly IMediator _mediator;
        private readonly IStoreMapper _storeMapper;
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;

        public EntityEventHandler(IMediator mediator, IStoreMapper storeMapper, IWorkflowDefinitionRepository workflowDefinitionRepository, IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository)
        {
            _mediator = mediator;
            _storeMapper = storeMapper;
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
        }

        public async Task HandleEventAsync(EntityCreatedEventData<WorkflowDefinitionVersion> eventData)
        {
            var definitionId = eventData.Entity.DefinitionId;
            var definition = await _workflowDefinitionRepository.GetAsync(definitionId);

            var model = _storeMapper.MapToModel(eventData.Entity, definition);

            await _mediator.Publish(new WorkflowDefinitionSaved(model));

            if (model.IsPublished)
                await _mediator.Publish(new WorkflowDefinitionPublished(model));
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<WorkflowDefinitionVersion> eventData)
        {
            var definitionId = eventData.Entity.DefinitionId;
            var definition = await _workflowDefinitionRepository.GetAsync(definitionId);

            var model = _storeMapper.MapToModel(eventData.Entity, definition);

            await _mediator.Publish(new WorkflowDefinitionSaved(model));

            if (model.IsPublished)
                await _mediator.Publish(new WorkflowDefinitionPublished(model));
        }

        public async Task HandleEventAsync(EntityDeletedEventData<WorkflowDefinitionVersion> eventData)
        {
            await _mediator.Publish(new WorkflowDefinitionDeleted(new Elsa.Models.WorkflowDefinition()
            {
                Id = eventData.Entity.Id.ToString(),
                DefinitionId = eventData.Entity.DefinitionId.ToString(),
                Version = eventData.Entity.Version
            }));
        }

        public async Task HandleEventAsync(EntityDeletedEventData<WorkflowInstance> eventData)
        {
            var model = _storeMapper.MapToModel(eventData.Entity);

            await _mediator.Publish(new WorkflowInstanceDeleted(model));
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<WorkflowDefinition> eventData)
        {
            var definitionId = eventData.Entity.Id;

            if (!eventData.Entity.PublishedVersion.HasValue)
            {
                var version = eventData.Entity.LatestVersion;
                var versionEntity = await _workflowDefinitionVersionRepository.GetByVersionAsync(definitionId, version);

                var model = _storeMapper.MapToModel(versionEntity, eventData.Entity);

                await _mediator.Publish(new WorkflowDefinitionDeleted(model));
            }
            else
            {
                var version = eventData.Entity.PublishedVersion.Value;
                var versionEntity = await _workflowDefinitionVersionRepository.GetByVersionAsync(definitionId, version);

                var model = _storeMapper.MapToModel(versionEntity, eventData.Entity);

                await _mediator.Publish(new WorkflowDefinitionSaved(model));
                await _mediator.Publish(new WorkflowDefinitionPublished(model));
            }
        }
    }
}
