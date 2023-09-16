using System;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public class WorkflowInstanceDateCountStatisticsRequestDto
{
    public int? DatePeriod { get; set; } = 30;
    public double Tz { get; set; }
    public Guid? WorkflowDefinitionId { get; set; }
}
