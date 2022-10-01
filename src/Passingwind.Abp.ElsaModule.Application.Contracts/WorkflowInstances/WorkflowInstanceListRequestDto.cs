using System;
using Elsa.Models;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceListRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public int? Version { get; set; }
        public WorkflowStatus? WorkflowStatus { get; set; }
        public string CorrelationId { get; set; }
        public Guid? WorkflowDefinitionId { get; set; }
    }
}
