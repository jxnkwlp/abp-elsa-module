using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public class WorkflowInstanceFaultBasicDto
{
    public Guid? FaultedActivityId { get; set; }
    public bool Resuming { get; set; }
    public object ActivityInput { get; set; }
    public string Message { get; set; }
    public SimpleExceptionModel Exception { get; set; }
}

public class WorkflowInstanceFaultDto : CreationAuditedEntityDto<Guid>
{
    public Guid WorkflowInstanceId { get; set; }
    public Guid? FaultedActivityId { get; set; }
    public bool Resuming { get; set; }
    public object ActivityInput { get; set; }
    public string Message { get; set; }
    public SimpleExceptionModel Exception { get; set; }
}