using System;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamAssignableUserListRequestDto : PagedResultRequestDto
{
    public string Filter { get; set; }
    public Guid? RoleId { get; set; }
}
