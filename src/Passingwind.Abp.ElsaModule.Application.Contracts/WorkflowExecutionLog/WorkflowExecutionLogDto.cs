using System;
using Newtonsoft.Json.Linq;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowExecutionLogDto : CreationAuditedEntityDto<long>
    {
        public Guid WorkflowInstanceId { get; set; }
        public long ActivityId { get; set; }
        public string ActivityType { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventName { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public JObject Data { get; set; }
    }
}
