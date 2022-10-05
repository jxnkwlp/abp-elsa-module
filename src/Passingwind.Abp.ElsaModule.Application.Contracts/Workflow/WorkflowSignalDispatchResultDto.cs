using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public class WorkflowSignalDispatchResultDto
    {
        public List<WorkflowSignalResultDto> StartedWorkflows { get; set; }
    }

    public class WorkflowSignalResultDto
    {
        public Guid WorkflowInstanceId { get; set; }
        public Guid? ActivityId { get; set; }
    }
}
