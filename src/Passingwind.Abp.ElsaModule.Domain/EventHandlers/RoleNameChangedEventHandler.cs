using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;

namespace Passingwind.Abp.ElsaModule.EventHandlers;

public class RoleNameChangedEventHandler : IDistributedEventHandler<IdentityRoleNameChangedEto>, ITransientDependency
{
    protected IWorkflowGroupRepository WorkflowGroupRepository { get; }

    public RoleNameChangedEventHandler(IWorkflowGroupRepository workflowGroupRepository)
    {
        WorkflowGroupRepository = workflowGroupRepository;
    }

    public async Task HandleEventAsync(IdentityRoleNameChangedEto eventData)
    {
        var roleId = eventData.Id;
        var newName = eventData.Name;

        var list = await WorkflowGroupRepository.GetListAsync(x => x.RoleId == roleId);

        foreach (var group in list)
        {
            group.RoleName = newName;
            await WorkflowGroupRepository.UpdateAsync(group);
        }
    }
}
