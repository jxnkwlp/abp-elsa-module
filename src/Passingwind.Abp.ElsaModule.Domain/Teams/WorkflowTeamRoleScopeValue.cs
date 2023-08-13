using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamRoleScopeValue : IEquatable<WorkflowTeamRoleScopeValue>
{
    public const string WorkflowProviderName = "Workflow";
    public const string WorkflowGroupProviderName = "WorkflowGroup";

    public string ProviderName { get; set; }
    public string ProviderValue { get; set; }


    public override string ToString()
    {
        return $"{ProviderName}: {ProviderValue}";
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as WorkflowTeamRoleScopeValue);
    }

    public bool Equals(WorkflowTeamRoleScopeValue other)
    {
        return other is not null &&
               ProviderName == other.ProviderName &&
               ProviderValue == other.ProviderValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ProviderName, ProviderValue);
    }

    public static bool operator ==(WorkflowTeamRoleScopeValue left, WorkflowTeamRoleScopeValue right)
    {
        return EqualityComparer<WorkflowTeamRoleScopeValue>.Default.Equals(left, right);
    }

    public static bool operator !=(WorkflowTeamRoleScopeValue left, WorkflowTeamRoleScopeValue right)
    {
        return !(left == right);
    }
}
