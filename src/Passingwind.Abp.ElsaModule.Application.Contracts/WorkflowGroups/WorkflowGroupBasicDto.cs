using System;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

public class WorkflowGroupBasicDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
}
