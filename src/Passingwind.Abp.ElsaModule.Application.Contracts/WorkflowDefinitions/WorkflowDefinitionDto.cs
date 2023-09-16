using System;
using System.Collections.Generic;
using Elsa.Models;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionDto : AuditedEntityDto<Guid>
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

    public Guid? GroupId { get; set; }

    public string GroupName { get; set; }

    public WorkflowPersistenceBehavior PersistenceBehavior { get; set; }

    public WorkflowContextOptions ContextOptions { get; set; }

    public Dictionary<string, object> Variables { get; set; }

    public Dictionary<string, object> CustomAttributes { get; set; }
}
