using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamRoleScopeDto : EntityDto
{
    public string RoleName { get; set; }

    public List<WorkflowTeamRoleScopeValueDto> Values { get; }
}
