namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class WorkflowInstanceGetStatusDateCountStatisticsRequestDto
    {
        public int? DatePeriod { get; set; } = 30;
        public double Tz { get; set; }
    }
}
