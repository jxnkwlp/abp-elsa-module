using System;
using Passingwind.Abp.ElsaModule.Common;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public class WorkflowInstanceFaultDto
{
    public Guid? FaultedActivityId { get; set; }
    public bool Resuming { get; set; }
    public object ActivityInput { get; set; }
    public string Message { get; set; }
    public SimpleExceptionModel Exception { get; set; }
}
