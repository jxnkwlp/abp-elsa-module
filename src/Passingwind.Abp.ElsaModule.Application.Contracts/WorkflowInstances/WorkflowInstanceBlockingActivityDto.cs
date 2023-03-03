using System;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public class WorkflowInstanceBlockingActivityDto
{
    public Guid ActivityId { get; set; }
    public string ActivityType { get; set; }
    public string Tag { get; set; }
}
