using System;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Groups;

public class WorkflowGroupDto : ExtensibleAuditedEntityDto<Guid>
{
    public virtual string Name { get; set; }
    public virtual string Description { get; set; }
    public virtual Guid? TenantId { get; set; }
}
