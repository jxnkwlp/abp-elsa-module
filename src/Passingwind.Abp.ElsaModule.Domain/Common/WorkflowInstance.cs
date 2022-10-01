using System;
using System.Collections.Generic;
using Elsa.Models;
using Elsa.Services.Models;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowInstance : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    protected WorkflowInstance()
    {
    }

    public WorkflowInstance(Guid id) : base(id)
    {
    }

    public Guid WorkflowDefinitionId { get; set; }

    public Guid WorkflowDefinitionVersionId { get; set; }

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


    public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

    public WorkflowInputReference Input { get; set; }

    public WorkflowOutputReference Output { get; set; }

    public WorkflowFault Fault { get; set; }

    public List<ScheduledActivity> ScheduledActivities { get; set; } = new List<ScheduledActivity>();

    public List<BlockingActivity> BlockingActivities { get; set; } = new List<BlockingActivity>();

    public List<ActivityScope> Scopes { get; set; } = new List<ActivityScope>();

    public ScheduledActivity CurrentActivity { get; set; }

    public Guid? LastExecutedActivityId { get; set; }

    public Dictionary<Guid, IDictionary<string, object>> ActivityData { get; set; } = new Dictionary<Guid, IDictionary<string, object>>();

    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

}
