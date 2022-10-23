using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Events;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Bookmarks;

namespace Passingwind.Abp.ElsaModule.EventHanders
{
    public class WorkflowFaultedNotificationHandler : INotificationHandler<WorkflowExecutionFinished>
    {
        private const string _activityType = nameof(Passingwind.Abp.ElsaModule.Activities.Workflows.WorkflowFaulted);
        private readonly ILogger<WorkflowFaultedNotificationHandler> _logger;
        private readonly IWorkflowLaunchpad _workflowLaunchpad;

        public WorkflowFaultedNotificationHandler(ILogger<WorkflowFaultedNotificationHandler> logger, IWorkflowLaunchpad workflowLaunchpad)
        {
            _logger = logger;
            _workflowLaunchpad = workflowLaunchpad;
        }

        public async Task Handle(WorkflowExecutionFinished notification, CancellationToken cancellationToken)
        {
            var instance = notification.WorkflowExecutionContext.WorkflowInstance;
            var blueprint = notification.WorkflowExecutionContext.WorkflowBlueprint;

            if (blueprint.Activities.Any(x => x.Type == _activityType))
                return;

            await RunWorkflowFaultedAsync(instance, blueprint, cancellationToken);
        }

        protected virtual async Task RunWorkflowFaultedAsync(WorkflowInstance instance, IWorkflowBlueprint workflowBlueprint, CancellationToken cancellationToken)
        {
            var bookmark = new WorkflowFaultedBookmark(workflowBlueprint.Name, workflowBlueprint.Version);
            var collectWorkflowsContext = new WorkflowsQuery(_activityType, bookmark, instance.CorrelationId, default, default, instance.TenantId);
            var pendingWorkflows = await _workflowLaunchpad.FindWorkflowsAsync(collectWorkflowsContext, cancellationToken);

            if (!pendingWorkflows.Any())
            {
                bookmark = new WorkflowFaultedBookmark(workflowBlueprint.Name);
                collectWorkflowsContext = new WorkflowsQuery(_activityType, bookmark, instance.CorrelationId, default, default, instance.TenantId);
                pendingWorkflows = await _workflowLaunchpad.FindWorkflowsAsync(collectWorkflowsContext, cancellationToken);
            }

            if (!pendingWorkflows.Any())
                return;

            var inputObj = new Activities.Workflows.WorkflowFaultedInput
            {
                WorkflowId = instance.Id,
                WorkflowName = instance.Name,
                WorkflowVersion = instance.Version,
                TenantId = instance.TenantId,
                CurrentActivity = instance.CurrentActivity,
                Fault = instance.Fault,
                Input = instance.Input,
                Variables = instance.Variables,
            };

            foreach (var workflow in pendingWorkflows)
            {
                var result = await _workflowLaunchpad.ExecutePendingWorkflowAsync(workflow, new WorkflowInput(inputObj), cancellationToken);

                // no-op
            }
        }
    }
}
