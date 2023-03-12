using System;
using Microsoft.AspNetCore.Authorization;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowAuthorizationRequirement : IAuthorizationRequirement
{
    public Guid Id { get; }
    public string Name { get; }

    public WorkflowAuthorizationRequirement(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}