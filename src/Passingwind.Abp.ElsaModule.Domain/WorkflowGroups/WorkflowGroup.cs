using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

/// <summary>
///  see https://github.com/jxnkwlp/abp-elsa-module/issues/10
/// </summary>
public class WorkflowGroup : FullAuditedAggregateRoot<Guid>, IEquatable<WorkflowGroup>, IMultiTenant
{
    public string Name { get; set; }

    public string Description { get; set; }

    public Guid RoleId { get; set; }

    public string RoleName { get; set; }

    public List<WorkflowGroupUser> Users { get; set; }

    public Guid? TenantId { get; set; }

    public WorkflowGroup(Guid id) : base(id)
    {
        Users = new List<WorkflowGroupUser>();
    }

    public WorkflowGroup(Guid id, string name, string description, Guid roleId, string roleName) : base(id)
    {
        Name = name;
        Description = description;
        RoleId = roleId;
        RoleName = roleName;
        Users = new List<WorkflowGroupUser>();
    }

    public string GetPermissionKey()
    {
        return Id.ToString("d");
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as WorkflowGroup);
    }

    public bool Equals(WorkflowGroup other)
    {
        return other is not null &&
               Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public static bool operator ==(WorkflowGroup left, WorkflowGroup right)
    {
        return EqualityComparer<WorkflowGroup>.Default.Equals(left, right);
    }

    public static bool operator !=(WorkflowGroup left, WorkflowGroup right)
    {
        return !(left == right);
    }
}
