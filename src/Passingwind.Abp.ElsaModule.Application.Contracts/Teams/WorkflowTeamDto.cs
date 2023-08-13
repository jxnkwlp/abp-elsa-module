using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<Guid> UserIds { get; set; }
}
