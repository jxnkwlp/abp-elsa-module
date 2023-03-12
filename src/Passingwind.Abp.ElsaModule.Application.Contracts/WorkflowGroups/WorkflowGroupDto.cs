using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

public class WorkflowGroupDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    public IEnumerable<Guid> UserIds { get; set; }
    public IEnumerable<Guid> WorkflowIds { get; set; }
}
