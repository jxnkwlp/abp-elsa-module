namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceStatusCountStatisticsResultDto
    {
        public long Running { get; set; }
        public long Finished { get; set; }
        public long Faulted { get; set; }
        public long Suspended { get; set; }
    }
}
