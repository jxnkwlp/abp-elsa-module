using System.Threading.Tasks;
using Elsa.Events;
using MediatR;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Stores;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Passingwind.Abp.ElsaModule.EventHandlers;

public class WorkflowInstanceEventHander :
    ILocalEventHandler<EntityDeletedEventData<WorkflowInstance>>,
    // 
    ITransientDependency
{
    private readonly IMediator _mediator;
    private readonly IStoreMapper _storeMapper;

    public WorkflowInstanceEventHander(IMediator mediator, IStoreMapper storeMapper)
    {
        _mediator = mediator;
        _storeMapper = storeMapper;
    }

    public async Task HandleEventAsync(EntityDeletedEventData<WorkflowInstance> eventData)
    {
        var model = _storeMapper.MapToModel(eventData.Entity);

        await _mediator.Publish(new WorkflowInstanceDeleted(model));
    }
}
