using System;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public class ActivityConnectionCreateDto
    {
        public Guid SourceId { get; set; }
        public Guid TargetId { get; set; }
        public string Outcome { get; set; }
    }
}
