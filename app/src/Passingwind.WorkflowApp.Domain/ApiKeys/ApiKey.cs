using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Passingwind.WorkflowApp.ApiKeys;

public class ApiKey : FullAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public DateTime? ExpirationTime { get; set; }

    public string Secret { get; set; }
}
