using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamRoleScope : Entity
{
    public Guid WorkflowTeamId { get; set; }

    public string RoleName { get; set; }

    public List<WorkflowTeamRoleScopeValue> Values { get; set; }


    protected WorkflowTeamRoleScope()
    {
        Values = new List<WorkflowTeamRoleScopeValue>();
    }

    public WorkflowTeamRoleScope(string roleName)
    {
        RoleName = roleName;
        Values = new List<WorkflowTeamRoleScopeValue>();
    }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowTeamId, RoleName };
    }
}
