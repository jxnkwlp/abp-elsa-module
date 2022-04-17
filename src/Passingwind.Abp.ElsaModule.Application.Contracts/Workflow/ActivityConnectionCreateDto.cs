namespace Passingwind.Abp.ElsaModule.Workflow
{
    public class ActivityConnectionCreateDto
    {
        public long SourceId { get; set; }
        public long TargetId { get; set; }
        public string Outcome { get; set; }
    }
}
