using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Elsa.Models;

namespace Passingwind.Abp.ElsaModule.Workflow;

public class ActivityCreateOrUpdateDto
{
    public Guid ActivityId { get; set; }

    [Required]
    [MaxLength(32)]
    public string Type { get; set; }
    [Required]
    [MaxLength(32)]
    public string Name { get; set; }

    public string DisplayName { get; set; }
    public string Description { get; set; }

    public bool PersistWorkflow { get; set; }
    public bool LoadWorkflowContext { get; set; }
    public bool SaveWorkflowContext { get; set; }

    public Dictionary<string, object> Attributes { get; set; }

    public List<ActivityDefinitionProperty> Properties { get; set; }

    public Dictionary<string, string> PropertyStorageProviders { get; set; }
}
