using System;
using System.Collections.Generic;
using Elsa.Models;
using Elsa.Services.Models;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceDto : AuditedEntityDto<Guid>
    {
        public Guid WorkflowDefinitionId { get; set; }

        public Guid WorkflowDefinitionVersionId { get; set; }

        public string Name { get; set; }
        public int Version { get; set; }

        public int WorkflowStatus { get; set; }
        public string CorrelationId { get; set; }
        public string ContextType { get; set; }
        public string ContextId { get; set; }

        public DateTime? LastExecutedTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public DateTime? CancelledTime { get; set; }
        public DateTime? FaultedTime { get; set; }

        public Dictionary<string, object> Variables { get; set; }
        public WorkflowInputReference Input { get; set; }
        public WorkflowOutputReference Output { get; set; }
        public WorkflowFault Fault { get; set; }
        public List<ScheduledActivity> ScheduledActivities { get; set; }
        public List<BlockingActivity> BlockingActivities { get; set; }
        public List<ActivityScope> Scopes { get; set; }
        public ScheduledActivity CurrentActivity { get; set; }
        public Guid? LastExecutedActivityId { get; set; }
        public Dictionary<string, Dictionary<string, object>> ActivityData { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
