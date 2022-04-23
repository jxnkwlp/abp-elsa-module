using System;
using Newtonsoft.Json.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowExecutionLog : CreationAuditedEntity<long>, IMultiTenant
    {
        public WorkflowExecutionLog()
        {
        }
        public WorkflowExecutionLog(long id) : base(id)
        {
        }

        public WorkflowInstance WorkflowInstance { get; set; }

        public Guid WorkflowInstanceId { get; set; }

        public Guid? TenantId { get; protected set; }

        public Guid ActivityId { get; set; }

        public string ActivityType { get; set; }

        public DateTime Timestamp { get; set; }

        public string EventName { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public JObject Data { get; set; }
    }
}
