using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Elsa.Models;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionCreateOrUpdateDto
{
    [Required]
    [MaxLength(64)]
    public string Name { get; set; }
    [MaxLength(128)]
    public string DisplayName { get; set; }
    [MaxLength(256)]
    public string Description { get; set; }

    public bool IsSingleton { get; set; }

    public bool DeleteCompletedInstances { get; set; }

    [MaxLength(64)]
    public string Channel { get; set; }

    [MaxLength(64)]
    public string Tag { get; set; }

    public Guid? GroupId { get; set; }

    public WorkflowPersistenceBehavior PersistenceBehavior { get; set; } = WorkflowPersistenceBehavior.WorkflowBurst;

    public WorkflowContextOptions ContextOptions { get; set; } 
}
