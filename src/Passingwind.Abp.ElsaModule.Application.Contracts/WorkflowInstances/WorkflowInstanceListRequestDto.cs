using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceListRequestDto : PagedResultRequestDto
    {
        public Guid? WorkflowDefinitionId { get; set; }
        public string Name { get; set; }
        public int? Version { get; set; }
        public WorkflowInstanceStatus? WorkflowStatus { get; set; }
        public string CorrelationId { get; set; }

        public DateTime[] CreationTimes { get; set; }
        public DateTime[] FinishedTimes { get; set; }
        public DateTime[] LastExecutedTimes { get; set; }
        public DateTime[] FaultedTimes { get; set; }

        public string Sorting { get; set; }
    }
}
