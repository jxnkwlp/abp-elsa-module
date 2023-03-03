using System.Threading.Tasks;
using Elsa.Events;
using MediatR;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Stores;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Passingwind.Abp.ElsaModule.EventHandlers;

public class WorkflowDefinitionEventHander :
    ILocalEventHandler<EntityCreatedEventData<WorkflowDefinitionVersion>>,
    ILocalEventHandler<EntityUpdatedEventData<WorkflowDefinitionVersion>>,
    ILocalEventHandler<EntityDeletedEventData<WorkflowDefinitionVersion>>,
    // 
    ILocalEventHandler<EntityUpdatedEventData<WorkflowDefinition>>,
    // 
    ITransientDependency
{
    private readonly IMediator _mediator;
    private readonly IStoreMapper _storeMapper;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;

    public WorkflowDefinitionEventHander(IMediator mediator, IStoreMapper storeMapper, IWorkflowDefinitionRepository workflowDefinitionRepository, IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository)
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
        else
            await _mediator.Publish(new WorkflowDefinitionRetracted(new Elsa.Models.WorkflowDefinition() { DefinitionId = definitionId.ToString(), Id = eventData.Entity.Id.ToString() }));
    }

    public async Task HandleEventAsync(EntityDeletedEventData<WorkflowDefinitionVersion> eventData)
    {
        var definitionId = eventData.Entity.DefinitionId;
        var definition = new WorkflowDefinition(definitionId);

        var model = _storeMapper.MapToModel(eventData.Entity, definition);

        await _mediator.Publish(new WorkflowDefinitionDeleted(model));
    }

    public async Task HandleEventAsync(EntityUpdatedEventData<WorkflowDefinition> eventData)
    {
        var definition = eventData.Entity;
        var definitionId = definition.Id;

        // published version
        if (definition.PublishedVersion.HasValue)
        {
            var publishVersion = definition.PublishedVersion.Value;
            var publishtVersionEntity = await _workflowDefinitionVersionRepository.GetByVersionAsync(definitionId, publishVersion);

            var publishModel = _storeMapper.MapToModel(publishtVersionEntity, definition);

            await _mediator.Publish(new WorkflowDefinitionSaved(publishModel));
            await _mediator.Publish(new WorkflowDefinitionPublished(publishModel));
        }
        else
        {
            var workflowDefinitionRetracted = new WorkflowDefinitionRetracted(new Elsa.Models.WorkflowDefinition { DefinitionId = definitionId.ToString(), Id = definition.Id.ToString() });
            await _mediator.Publish(workflowDefinitionRetracted);
        }

        // latest version
        var latestVersion = definition.LatestVersion;
        var latestVersionEntity = await _workflowDefinitionVersionRepository.GetByVersionAsync(definitionId, latestVersion);

        var latestModel = _storeMapper.MapToModel(latestVersionEntity, definition);

        await _mediator.Publish(new WorkflowDefinitionSaved(latestModel));
    }
}
