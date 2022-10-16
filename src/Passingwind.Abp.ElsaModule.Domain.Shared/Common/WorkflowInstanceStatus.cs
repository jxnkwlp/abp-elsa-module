namespace Passingwind.Abp.ElsaModule.Common
{
    public enum WorkflowInstanceStatus
    {
        Idle = 0,
        Running,
        Finished,
        Suspended,
        Faulted,
        Cancelled
    }
}
