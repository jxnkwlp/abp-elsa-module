using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceListRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public int? Version { get; set; }
        public WorkflowInstanceStatus? WorkflowStatus { get; set; }
        public string CorrelationId { get; set; }
        public Guid? WorkflowDefinitionId { get; set; }
    }
}
