using System;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions
{
    public class WorkflowDefinitionDispatchRequestDto
    {
        public Guid? ActivityId { get; set; }
        [MaxLength(36)]
        public string CorrelationId { get; set; }
        [MaxLength(36)]
        public string ContextId { get; set; }
        public object Input { get; set; }
    }
}