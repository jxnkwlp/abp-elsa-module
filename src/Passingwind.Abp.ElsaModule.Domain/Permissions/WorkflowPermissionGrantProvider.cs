namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowPermissionGrantProvider
{
    public WorkflowPermissionGrantProvider(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }

    public override string ToString()
    {
        return $"{Name}, {Value}";
    }
}
