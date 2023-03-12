using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.EventHandlers;

public class UserDeleteEventHander : IDistributedEventHandler<EntityDeletedEventData<IdentityUser>>, ITransientDependency
{
    private readonly PermissionManager _permissionManager;

    public UserDeleteEventHander(PermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    public async Task HandleEventAsync(EntityDeletedEventData<IdentityUser> eventData)
    {
        await _permissionManager.DeleteAsync(WorkflowUserOwnerPermissionValueProvider.ProviderName, eventData.Entity.Id.ToString("d"));
    }
}
