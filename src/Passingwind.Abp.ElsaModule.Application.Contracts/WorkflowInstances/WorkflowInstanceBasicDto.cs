using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public class WorkflowInstanceBasicDto : AuditedEntityDto<Guid>
{
    public Guid WorkflowDefinitionId { get; set; }

    public Guid WorkflowDefinitionVersionId { get; set; }

    public string Name { get; set; }
    public int Version { get; set; }

    public WorkflowInstanceStatus WorkflowStatus { get; set; }
    public string CorrelationId { get; set; }
    public string ContextType { get; set; }
    public string ContextId { get; set; }

    public Guid? GroupId { get; set; }

    public DateTime? LastExecutedTime { get; set; }
    public DateTime? FinishedTime { get; set; }
    public DateTime? CancelledTime { get; set; }
    public DateTime? FaultedTime { get; set; }
    public TimeSpan? FinishedDuration { get; set; }
}
