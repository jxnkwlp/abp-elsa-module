using System;
using Elsa.Models;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceBasicDto : AuditedEntityDto<Guid>
    {
        public Guid WorkflowDefinitionId { get; set; }

        public Guid WorkflowDefinitionVersionId { get; set; }

        public string Name { get; set; }
        public int Version { get; set; }

        // public WorkflowStatus WorkflowStatus { get; set; }
        public int WorkflowStatus { get; set; }
        public string CorrelationId { get; set; }
        public string ContextType { get; set; }
        public string ContextId { get; set; }

        public DateTime? LastExecutedTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public DateTime? CancelledTime { get; set; }
        public DateTime? FaultedTime { get; set; }

        public WorkflowFault Fault { get; set; }
    }
}