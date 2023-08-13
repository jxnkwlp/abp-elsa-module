using System.Collections.Generic;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp.Identity;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionIamResultDto
{
    public IEnumerable<IdentityUserDto> Owners { get; set; }
    public IEnumerable<WorkflowTeamBasicDto> Teams { get; set; }
}
