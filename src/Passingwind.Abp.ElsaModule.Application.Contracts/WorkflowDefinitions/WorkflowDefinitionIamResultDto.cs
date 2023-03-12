using System.Collections.Generic;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
using Volo.Abp.Identity;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionIamResultDto
{
    public IEnumerable<IdentityUserDto> Owners { get; set; }
    public IEnumerable<WorkflowGroupBasicDto> Groups { get; set; }
}
