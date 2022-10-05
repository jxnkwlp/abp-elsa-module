using System;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public class WorkflowSignalDispatchRequestDto
    { 
        public Guid? WorkflowInstanceId { get; set; }
        public string CorrelationId { get; set; }
        public object Input { get; set; }
    }
}
