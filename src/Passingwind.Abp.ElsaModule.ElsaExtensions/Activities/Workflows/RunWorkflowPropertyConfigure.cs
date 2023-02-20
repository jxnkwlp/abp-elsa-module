using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Workflows;
using Elsa.Design;
using Elsa.Events;
using MediatR;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Activities.Workflows;

public class RunWorkflowPropertyConfigure : INotificationHandler<DescribingActivityType>
{
    public Task Handle(DescribingActivityType notification, CancellationToken cancellationToken)
    {
        var activityType = notification.ActivityType;

        if (activityType.Type != typeof(RunWorkflow))
            return Task.CompletedTask;

        var inputProperties = notification.ActivityDescriptor.InputProperties.ToList();

        var workflowDefinitionProperty = inputProperties.First(x => x.Name == nameof(RunWorkflow.WorkflowDefinitionId));

        workflowDefinitionProperty.Options = new RuntimeSelectListProviderSettings(typeof(RunWorkflowWorkflowDefinitionIdRuntimeSelectListProvider));

        return Task.CompletedTask;
    }
}

public class RunWorkflowWorkflowDefinitionIdRuntimeSelectListProvider : IRuntimeSelectListProvider, ITransientDependency
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;

    public RunWorkflowWorkflowDefinitionIdRuntimeSelectListProvider(IWorkflowDefinitionRepository workflowDefinitionRepository)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
    }

    public async ValueTask<SelectList> GetSelectListAsync(object context = null, CancellationToken cancellationToken = default)
    {
        var list = await _workflowDefinitionRepository.GetListAsync(includeDetails: false);

        return new SelectList(list.OrderBy(x => x.Name).Select(x => new SelectListItem($"{x.DisplayName}({x.Name})", x.Id.ToString())).ToArray());
    }
}
