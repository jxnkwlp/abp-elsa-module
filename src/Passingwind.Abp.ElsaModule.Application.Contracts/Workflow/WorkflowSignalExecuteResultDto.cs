using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public class WorkflowSignalExecuteResultDto
    {
        public List<WorkflowSignalResultDto> StartedWorkflows { get; set; }
    }
}
