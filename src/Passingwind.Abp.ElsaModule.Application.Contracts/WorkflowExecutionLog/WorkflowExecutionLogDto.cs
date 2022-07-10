using System;
using Newtonsoft.Json.Linq;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowExecutionLogDto : CreationAuditedEntityDto<Guid>
    {
        public Guid WorkflowInstanceId { get; set; }
        public Guid ActivityId { get; set; }
        public string ActivityType { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventName { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public JObject Data { get; set; }
    }
}
