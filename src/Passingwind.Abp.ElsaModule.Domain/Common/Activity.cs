using System;
using System.Collections.Generic;
using Elsa.Models;
using Volo.Abp.Domain.Entities.Auditing;

namespace Passingwind.Abp.ElsaModule.Common;

public class Activity : AuditedEntity
{
    public Guid DefinitionVersionId { get; set; }

    public Guid ActivityId { get; set; }

    public string Type { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }

    public bool PersistWorkflow { get; set; }
    public bool LoadWorkflowContext { get; set; }
    public bool SaveWorkflowContext { get; set; }

    public Dictionary<string, object> Arrtibutes { get; set; }

    public List<ActivityDefinitionProperty> Properties { get; set; }

    public Dictionary<string, string> PropertyStorageProviders { get; set; }

    public Activity()
    {
        Arrtibutes = new Dictionary<string, object>();
    }

    public override object[] GetKeys()
    {
        return new object[] { DefinitionVersionId, ActivityId };
    }
}
