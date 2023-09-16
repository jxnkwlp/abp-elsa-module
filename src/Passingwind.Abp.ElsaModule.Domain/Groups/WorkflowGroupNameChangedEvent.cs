using System;

namespace Passingwind.Abp.ElsaModule.Groups;

public class WorkflowGroupNameChangedEvent {
    public WorkflowGroupNameChangedEvent(string oldName, string name, Guid id)
    {
        OldName = oldName;
        Name = name;
        Id = id;
    }

    public string OldName { get; }
    public string Name { get; }
    public Guid Id { get; }
}
