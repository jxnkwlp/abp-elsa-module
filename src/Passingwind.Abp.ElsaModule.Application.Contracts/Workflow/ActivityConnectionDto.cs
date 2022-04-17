using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public class ActivityConnectionDto : CreationAuditedEntityDto
    {
        public long SourceId { get; set; }
        public long TargetId { get; set; }
        public string Outcome { get; set; }
    }
}
