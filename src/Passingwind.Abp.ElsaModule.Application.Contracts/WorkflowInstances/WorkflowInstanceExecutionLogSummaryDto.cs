using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.WorkflowInstances;

public class WorkflowInstanceExecutionLogSummaryDto
{
    public List<WorkflowInstanceExecutionLogSummaryActivityDto> Activities { get; set; }
}

public class WorkflowInstanceExecutionLogSummaryActivityDto
{
    public Guid ActivityId { get; set; }
    /// <summary>
    /// public string Name { get; set; }
    /// public string DisplayName { get; set; }
    /// </summary>
    public string ActivityType { get; set; }
    public bool IsExecuting { get; set; }
    public bool IsExecuted { get; set; }
    public bool IsFaulted { get; set; }
    public int ExecutedCount { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double Duration { get; set; }
    public string[] Outcomes { get; set; }
    public Dictionary<string, object> StateData { get; set; }
    public Dictionary<string, object> JournalData { get; set; }
    public string Message { get; set; }
}
