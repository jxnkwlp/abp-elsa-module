using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowExecutionLog : CreationAuditedAggregateRoot<Guid>, IMultiTenant
{
    public WorkflowExecutionLog()
    {
    }

    public WorkflowExecutionLog(Guid id) : base(id)
    {
    }

    public Guid WorkflowInstanceId { get; set; }

    public Guid? TenantId { get; protected set; }

    public Guid ActivityId { get; set; }

    public string ActivityType { get; set; }

    public DateTime Timestamp { get; set; }

    public string EventName { get; set; }

    public string Message { get; set; }

    public string Source { get; set; }

    public Dictionary<string, object> Data { get; set; }
}
