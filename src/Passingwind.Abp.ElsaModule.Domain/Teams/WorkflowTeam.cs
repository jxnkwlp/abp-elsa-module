using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Teams;

/// <summary>
///  see https://github.com/jxnkwlp/abp-elsa-module/issues/10
/// </summary>
public class WorkflowTeam : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual string Name { get; protected set; }

    public virtual string Description { get; set; }

    public virtual List<WorkflowTeamUser> Users { get; protected set; }

    public virtual List<WorkflowTeamRoleScope> RoleScopes { get; protected set; }

    public virtual Guid? TenantId { get; set; }

    protected WorkflowTeam()
    {
        Users = new List<WorkflowTeamUser>();
        RoleScopes = new List<WorkflowTeamRoleScope>();
    }

    public WorkflowTeam(Guid id, string name, string description = null) : base(id)
    {
        Name = name;
        Description = description;
        Users = new List<WorkflowTeamUser>();
        RoleScopes = new List<WorkflowTeamRoleScope>();
    }

    public virtual string GetPermissionKey()
    {
        return Id.ToString("d");
    }

    public virtual void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        Name = name;
    }
}
