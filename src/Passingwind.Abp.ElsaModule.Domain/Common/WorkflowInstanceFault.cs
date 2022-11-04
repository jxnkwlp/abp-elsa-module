using System;
using Elsa.Models;
using Volo.Abp.Domain.Entities.Auditing;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowInstanceFault : CreationAuditedEntity<Guid>
    {
        public Guid WorkflowInstanceId { get; set; }
        public Guid? FaultedActivityId { get; set; }
        public bool Resuming { get; set; }
        public object ActivityInput { get; set; }
        public string Message { get; set; }
        public SimpleException Exception { get; set; }
    }
}
