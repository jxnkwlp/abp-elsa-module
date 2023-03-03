using System;
using Elsa.Models;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public class WorkflowInstanceExecuteRequestDto
{
    public Guid? ActivityId { get; set; }
    public WorkflowInput Input { get; set; }
}
