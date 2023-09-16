using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowExecutionLog;

public class WorkflowExecutionLogDto : CreationAuditedEntityDto<Guid>
{
    public Guid WorkflowInstanceId { get; set; }
    public Guid ActivityId { get; set; }
    public string ActivityType { get; set; }
    public DateTime Timestamp { get; set; }
    public string EventName { get; set; }
    public string Message { get; set; }
    public string Source { get; set; }
    public Dictionary<string, object> Data { get; set; }
}
