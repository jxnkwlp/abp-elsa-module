namespace Passingwind.Abp.ElsaModule.WorkflowInstances
{
    public class GetStatusDateCountStatisticsRequestDto
    {
        public int? DatePeriod { get; set; } = 30;
        public double Tz { get; set; }
    }
}
