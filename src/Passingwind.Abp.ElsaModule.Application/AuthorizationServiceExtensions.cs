using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Permissions;

namespace Passingwind.Abp.ElsaModule;

public static class AuthorizationServiceExtensions
{
    public static async Task CheckAsync(this IAuthorizationService authorizationService, WorkflowDefinition workflow, string name)
    {
        await authorizationService.CheckAsync(new WorkflowDefinition(workflow.Id), new WorkflowAuthorizationRequirement(workflow.Id, name));
    }
}
