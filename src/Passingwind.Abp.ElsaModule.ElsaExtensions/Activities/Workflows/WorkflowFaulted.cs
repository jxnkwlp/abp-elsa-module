using System.ComponentModel.DataAnnotations;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Passingwind.Abp.ElsaModule.Activities.Workflows;

[Trigger(
    Category = "Workflows",
    DisplayName = "Workflow Faulted",
    Description = "Triggers when workflow status change to faulted.",
    Outcomes = new[] { OutcomeNames.Done }
)]
public class WorkflowFaulted : Activity
{
    private readonly ILogger<WorkflowFaulted> _logger;
    private readonly IClock _clock;

    public WorkflowFaulted(ILogger<WorkflowFaulted> logger, IClock clock)
    {
        _logger = logger;
        _clock = clock;
    }

    [ActivityInput(
        UIHint = ActivityInputUIHints.SingleLine,
        Hint = "Workflow definition name",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.Liquid, SyntaxNames.JavaScript, },
        IsDesignerCritical = true
    )]
    [Required]
    public string DefinitionName { get; set; }

    [ActivityInput(
        UIHint = ActivityInputUIHints.SingleLine,
        Hint = "Workflow definition version",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.Liquid, SyntaxNames.JavaScript }
    )]
    public int? DefinitionVersion { get; set; }

    [ActivityOutput]
    public WorkflowFaultedInput Output { get; set; }

    protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
    {
        var now = _clock.GetCurrentInstant();

        context.JournalData["Trigger At"] = now;

        Output = context.GetInput<WorkflowFaultedInput>();

        return Done();
    }

}

public class WorkflowFaultedInput
{
    public string WorkflowId { get; set; }
    public string WorkflowName { get; set; }
    public int WorkflowVersion { get; set; }
    public string TenantId { get; set; }
    public WorkflowInputReference Input { get; set; }
    public WorkflowFault[] Faults { get; set; }
    public WorkflowFault Fault { get; set; }
    public Variables Variables { get; set; }
    public ScheduledActivity CurrentActivity { get; set; }
    public IActivityBlueprint FaultedActivity { get; set; }
}
