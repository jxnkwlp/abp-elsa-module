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

    [Obsolete]
    public WorkflowFault Fault { get; set; }

    public List<WorkflowInstanceFault> Faults { get; set; } = new List<WorkflowInstanceFault>();

    public WorkflowInstanceScheduledActivity CurrentActivity { get; set; }

    public List<WorkflowInstanceVariable> Variables { get; set; } = new List<WorkflowInstanceVariable>();

    public List<WorkflowInstanceMetadata> Metadata { get; set; } = new List<WorkflowInstanceMetadata>();

    public List<WorkflowInstanceScheduledActivity> ScheduledActivities { get; set; } = new List<WorkflowInstanceScheduledActivity>();

    public List<WorkflowInstanceBlockingActivity> BlockingActivities { get; set; } = new List<WorkflowInstanceBlockingActivity>();

    public List<WorkflowInstanceActivityScope> ActivityScopes { get; set; } = new List<WorkflowInstanceActivityScope>();

    public List<WorkflowInstanceActivityData> ActivityData { get; set; } = new List<WorkflowInstanceActivityData>();


    public IDictionary<string, object> GetMetadata()
    {
        var dict = new Dictionary<string, object>();
        foreach (var item in Metadata)
        {
            dict[item.Key] = item.Value;
        }
        return dict;
    }

    public IDictionary<string, object> GetVariables()
    {
        var dict = new Dictionary<string, object>();
        foreach (var item in Variables)
        {
            dict[item.Key] = item.Value;
        }
        return dict;
    }
}
