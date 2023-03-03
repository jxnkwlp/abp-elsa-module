using System;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionVersionListItemDto : AuditedEntityDto<Guid>
{
    public int Version { get; set; }

    public bool IsLatest { get; set; }

    public bool IsPublished { get; set; }
}
