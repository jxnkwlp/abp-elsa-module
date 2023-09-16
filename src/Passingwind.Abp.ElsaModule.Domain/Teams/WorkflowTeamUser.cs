using System;
using Volo.Abp.Domain.Entities;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamUser : Entity
{
    public Guid WorkflowTeamId { get; set; }

    public Guid UserId { get; set; }

    protected WorkflowTeamUser()
    {
    }

    public WorkflowTeamUser(Guid userId)
    {
        UserId = userId;
    }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowTeamId, UserId };
    }
}
