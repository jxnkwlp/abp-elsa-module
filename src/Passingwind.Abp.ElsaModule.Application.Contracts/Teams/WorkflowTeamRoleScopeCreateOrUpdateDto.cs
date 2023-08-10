using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamRoleScopeCreateOrUpdateDto
{
    [Required]
    [DynamicMaxLength(typeof(IdentityRoleConsts), nameof(IdentityRoleConsts.MaxNameLength))]
    public string RoleName { get; set; }

    public List<WorkflowTeamRoleScopeValueDto> Items { get; set; }
}
