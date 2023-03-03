using System;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowExecutionLogListRequestDto : PagedResultRequestDto
{
    public Guid? WorkflowInstanceId { get; set; }
    public Guid? ActivityId { get; set; }
}
