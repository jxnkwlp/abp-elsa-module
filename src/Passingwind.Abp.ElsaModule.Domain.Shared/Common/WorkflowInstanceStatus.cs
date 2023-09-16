namespace Passingwind.Abp.ElsaModule.Common;

public enum WorkflowInstanceStatus
{
    Idle = 0,
    Running = 1,
    Finished = 2,
    Suspended = 3,
    Faulted = 4,
    Cancelled = 5
}
