using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceActivityScopeDto
    {
        public Guid ActivityId { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }
}
