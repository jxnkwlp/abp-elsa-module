using System;

namespace Passingwind.Abp.ElsaModule.Workflow;

public class WorkflowSignalExecuteRequestDto
{
    public Guid? WorkflowInstanceId { get; set; }
    public string CorrelationId { get; set; }
    public object Input { get; set; }
}
