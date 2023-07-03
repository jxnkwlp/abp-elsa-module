using System;
using Elsa.Models;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionBasicDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public int LatestVersion { get; set; }

    public int? PublishedVersion { get; set; }

    public bool IsSingleton { get; set; }

    public bool DeleteCompletedInstances { get; set; }

    public string Channel { get; set; }

    public string Tag { get; set; }

    public WorkflowPersistenceBehavior PersistenceBehavior { get; set; }
}
