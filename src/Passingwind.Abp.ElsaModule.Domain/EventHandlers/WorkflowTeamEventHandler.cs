using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Permissions;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.EventHandlers;

public class WorkflowTeamEventHandler :
    ILocalEventHandler<EntityCreatedEventData<WorkflowTeam>>,
    ILocalEventHandler<EntityUpdatedEventData<WorkflowTeam>>,
    ILocalEventHandler<EntityDeletedEventData<WorkflowTeam>>,
    ITransientDependency,
    IUnitOfWorkEnabled
{
    private readonly PermissionManager _permissionManager;
    private readonly IWorkflowTeamManager _workflowTeamManager;

    public WorkflowTeamEventHandler(PermissionManager permissionManager, IWorkflowTeamManager workflowTeamManager)
    {
        _permissionManager = permissionManager;
        _workflowTeamManager = workflowTeamManager;
    }

    public Task HandleEventAsync(EntityCreatedEventData<WorkflowTeam> eventData)
    {
        return Task.CompletedTask;
    }

    public async Task HandleEventAsync(EntityDeletedEventData<WorkflowTeam> eventData)
    {
        var key = eventData.Entity.GetPermissionKey();

        await _permissionManager.DeleteAsync(WorkflowTeamPermissionValueProvider.ProviderName, key);
    }

    public async Task HandleEventAsync(EntityUpdatedEventData<WorkflowTeam> eventData)
    {
        var entity = eventData.Entity;

        await _workflowTeamManager.UpdatePermissionGrantsAsync(entity);
    }
}
