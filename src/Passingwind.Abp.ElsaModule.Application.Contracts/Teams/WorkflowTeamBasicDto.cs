using System;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamBasicDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
}
