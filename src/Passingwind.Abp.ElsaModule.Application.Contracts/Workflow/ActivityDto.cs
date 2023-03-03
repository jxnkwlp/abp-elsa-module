using System;
using System.Collections.Generic;
using Elsa.Models;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow;

public class ActivityDto : EntityDto
{
    public Guid ActivityId { get; set; }

    public string Type { get; set; }
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
