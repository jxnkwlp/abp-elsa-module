using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Permissions;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.EventHandlers;

public class WorkflowGroupEventHandler :
    IDistributedEventHandler<EntityCreatedEventData<WorkflowGroup>>,
    IDistributedEventHandler<EntityDeletedEventData<WorkflowGroup>>,
    ITransientDependency
{
    private readonly PermissionManager _permissionManager;
    private readonly IWorkflowGroupManager _workflowGroupManager;

    public WorkflowGroupEventHandler(PermissionManager permissionManager, IWorkflowGroupManager workflowGroupManager)
    {
        _permissionManager = permissionManager;
        _workflowGroupManager = workflowGroupManager;
    }

    public Task HandleEventAsync(EntityCreatedEventData<WorkflowGroup> eventData)
    {
        return Task.CompletedTask;
    }

    public async Task HandleEventAsync(EntityDeletedEventData<WorkflowGroup> eventData)
    {
        var key = eventData.Entity.GetPermissionKey();

        await _permissionManager.DeleteAsync(WorkflowGroupPermissionValueProvider.ProviderName, key);
    }
}
