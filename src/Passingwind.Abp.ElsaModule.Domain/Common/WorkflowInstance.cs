using System;
using System.Collections.Generic;
using Elsa.Models;
using Elsa.Services.Models;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowInstance : FullAuditedEntity<Guid>, IMultiTenant
{
    public WorkflowInstance()
    {
    }

    public WorkflowInstance(Guid id) : base(id)
    {
    }

    public Guid DefinitionId { get; set; }

    public Guid DefinitionVersionId { get; set; }

    public Guid? TenantId { get; protected set; }

    public string Name { get; set; }

    public int Version { get; set; }

    public WorkflowStatus WorkflowStatus { get; set; } = WorkflowStatus.Idle;

    public string CorrelationId { get; set; }

    public string ContextType { get; set; }

    public string ContextId { get; set; }

    public DateTime? LastExecutedTime { get; set; }
    public DateTime? FinishedTime { get; set; }
    public DateTime? CancelledTime { get; set; }
    public DateTime? FaultedTime { get; set; }


    public Dictionary<string, object> Variables { get; set; }

    public WorkflowInputReference Input { get; set; }

    public WorkflowOutputReference Output { get; set; }

    public WorkflowFault Fault { get; set; }

    public List<ScheduledActivity> ScheduledActivities { get; set; }

    public List<BlockingActivity> BlockingActivities { get; set; }

    public List<ActivityScope> Scopes { get; set; }

    public ScheduledActivity CurrentActivity { get; set; }

    public long? LastExecutedActivityId { get; set; }

    public Dictionary<string, IDictionary<string, object>> ActivityData { get; set; }

    public Dictionary<string, object> Metadata { get; set; }

}
