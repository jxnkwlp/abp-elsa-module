using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Groups;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.EventHandlers;

public class WorkflowGroupLocalEventHandler :
    ILocalEventHandler<WorkflowGroupNameChangedEvent>,
    ILocalEventHandler<EntityDeletedEventData<WorkflowGroup>>,
    ITransientDependency,
    IUnitOfWorkEnabled
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;

    public WorkflowGroupLocalEventHandler(IWorkflowDefinitionRepository workflowDefinitionRepository)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
    }

    public virtual async Task HandleEventAsync(WorkflowGroupNameChangedEvent eventData)
    {
        var groupId = eventData.Id;
        var newName = eventData.Name;

        await _workflowDefinitionRepository.UpdateGroupNameAsync(groupId, newName);
    }

    public async Task HandleEventAsync(EntityDeletedEventData<WorkflowGroup> eventData)
    {
        var groupId = eventData.Entity.Id;

        await _workflowDefinitionRepository.RemoveGroupAsync(groupId);
    }
}
