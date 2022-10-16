using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceActivityDataDto
    {
        public Guid ActivityId { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}
