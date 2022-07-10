using System;
using System.Collections.Generic;
using Elsa.Models;
using Entity = Volo.Abp.Domain.Entities.Entity;

namespace Passingwind.Abp.ElsaModule.Common;

public class Activity : Entity
{
    public Guid WorkflowDefinitionVersionId { get; set; }
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

    protected Activity()
    {
        Attributes ??= new Dictionary<string, object>();
        Properties ??= new List<ActivityDefinitionProperty>();
    }

    public Activity(Guid activityId, string type, string name, string displayName, string description, bool persistWorkflow, bool loadWorkflowContext, bool saveWorkflowContext, Dictionary<string, object> attributes, List<ActivityDefinitionProperty> properties, Dictionary<string, string> propertyStorageProviders) : this()
    {
        ActivityId = activityId;
        Type = type;
        Name = name;
        DisplayName = displayName;
        Description = description;
        PersistWorkflow = persistWorkflow;
        LoadWorkflowContext = loadWorkflowContext;
        SaveWorkflowContext = saveWorkflowContext;
        Attributes = attributes ?? new Dictionary<string, object>();
        Properties = properties ?? new List<ActivityDefinitionProperty>();
        PropertyStorageProviders = propertyStorageProviders ?? new Dictionary<string, string>();
    }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowDefinitionVersionId, ActivityId };
    }
}
