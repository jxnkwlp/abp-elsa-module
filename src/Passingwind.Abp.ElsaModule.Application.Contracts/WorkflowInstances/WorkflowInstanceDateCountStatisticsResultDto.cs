using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceDateCountStatisticsResultDto
    {
        public List<WorkflowInstanceDateCountStatisticDto> Items { get; set; }

        public WorkflowInstanceDateCountStatisticsResultDto()
        {
            Items = new List<WorkflowInstanceDateCountStatisticDto>();
        }
    }

    public class WorkflowInstanceDateCountStatisticDto
    {
        public DateTime Date { get; set; }
        public int FinishedCount { get; set; }
        public int FailedCount { get; set; }
    }
}
