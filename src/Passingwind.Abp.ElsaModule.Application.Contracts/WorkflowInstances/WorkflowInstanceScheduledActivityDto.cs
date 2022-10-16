using System;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceScheduledActivityDto
    {
        public Guid ActivityId { get; set; }
        public object Input { get; set; }
    }
}
