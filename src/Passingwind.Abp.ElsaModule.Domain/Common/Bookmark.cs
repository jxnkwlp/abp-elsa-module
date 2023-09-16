using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common;

public class Bookmark : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Bookmark()
    {
    }

    public Bookmark(Guid id) : base(id)
    {
    }

    public Guid WorkflowInstanceId { get; set; }

    public Guid? TenantId { get; protected set; }

    public Guid ActivityId { get; set; }
    public string ActivityType { get; set; }
    public string CorrelationId { get; set; }
    public string Hash { get; set; }
    public string ModelType { get; set; }
    public string Model { get; set; }
}
