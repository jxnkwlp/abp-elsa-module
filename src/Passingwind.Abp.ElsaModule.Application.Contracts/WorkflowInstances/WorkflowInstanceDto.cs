using System;
using System.Collections.Generic;
using Elsa.Services.Models;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public class WorkflowInstanceDto : AuditedEntityDto<Guid>
{
    public Guid WorkflowDefinitionId { get; set; }

    public Guid WorkflowDefinitionVersionId { get; set; }

    public string Name { get; set; }
    public int Version { get; set; }

    public WorkflowInstanceStatus WorkflowStatus { get; set; } = WorkflowInstanceStatus.Idle;

    public string CorrelationId { get; set; }
    public string ContextType { get; set; }
    public string ContextId { get; set; }

    public DateTime? LastExecutedTime { get; set; }
    public DateTime? FinishedTime { get; set; }
    public DateTime? CancelledTime { get; set; }
    public DateTime? FaultedTime { get; set; }

    public Guid? LastExecutedActivityId { get; set; }
    public WorkflowInputReference Input { get; set; }
    public WorkflowOutputReference Output { get; set; }
    public WorkflowInstanceScheduledActivityDto CurrentActivity { get; set; }
    public List<WorkflowInstanceFaultDto> Faults { get; set; }

    public Dictionary<string, object> Variables { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    public List<WorkflowInstanceScheduledActivityDto> ScheduledActivities { get; set; }
    public List<WorkflowInstanceBlockingActivityDto> BlockingActivities { get; set; }
    public List<WorkflowInstanceActivityScopeDto> ActivityScopes { get; set; }
    public List<WorkflowInstanceActivityDataDto> ActivityData { get; set; }
}
