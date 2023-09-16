using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Elsa.Models;
using Passingwind.Abp.ElsaModule.Groups;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowDefinition : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    protected WorkflowDefinition()
    {
        Variables = new Dictionary<string, object>();
        CustomAttributes = new Dictionary<string, object>();
    }

    [JsonConstructor]
    public WorkflowDefinition(Guid id) : base(id)
    {
        Variables = new Dictionary<string, object>();
        CustomAttributes = new Dictionary<string, object>();
    }

    public WorkflowDefinition(
        Guid id,
        string name,
        string displayName,
        Guid? tenantId,
        string description = null,
        bool isSingleton = false,
        bool deleteCompletedInstances = false,
        string channel = null,
        string tag = null,
        WorkflowPersistenceBehavior? persistenceBehavior = null,
        WorkflowContextOptions contextOptions = null,
        Dictionary<string, object> variables = null,
        Dictionary<string, object> customAttributes = null) : base(id)
    {
        Name = name;
        DisplayName = displayName;
        TenantId = tenantId;
        Description = description;
        IsSingleton = isSingleton;
        DeleteCompletedInstances = deleteCompletedInstances;
        Channel = channel;
        Tag = tag;
        PersistenceBehavior = persistenceBehavior ?? WorkflowPersistenceBehavior.WorkflowBurst;
        ContextOptions = contextOptions;
        Variables = variables ?? new Dictionary<string, object>();
        CustomAttributes = customAttributes ?? new Dictionary<string, object>();
    }

    public WorkflowDefinition(Guid id, string name, Guid? tenantId) : base(id)
    {
        Name = name;
        TenantId = tenantId;
        Variables = new Dictionary<string, object>();
        CustomAttributes = new Dictionary<string, object>();
    }

    public string Name { get; protected set; }

    public string DisplayName { get; set; }

    public Guid? TenantId { get; set; }

    public string Description { get; set; }

    public int LatestVersion { get; protected set; } = 1;

    public int? PublishedVersion { get; protected set; }

    public bool IsSingleton { get; set; }

    public bool DeleteCompletedInstances { get; set; }

    public string Channel { get; set; }

    public string Tag { get; set; }

    public Guid? GroupId { get; protected set; }
    public string GroupName { get; set; }

    public WorkflowPersistenceBehavior PersistenceBehavior { get; set; } = WorkflowPersistenceBehavior.WorkflowBurst;

    public WorkflowContextOptions ContextOptions { get; set; }

    public Dictionary<string, object> Variables { get; set; }

    public Dictionary<string, object> CustomAttributes { get; set; }

    public void SetVersion(int latestVersion, int? publishedVersion)
    {
        LatestVersion = latestVersion;
        PublishedVersion = publishedVersion;
    }

    public void SetLatestVersion(int version)
    {
        LatestVersion = version;
    }

    public void SetPublishedVersion(int? version)
    {
        PublishedVersion = version;
    }

    public void ChangeName(string name)
    {
        Name = name;
    }

    public void SetGroup(WorkflowGroup workflowGroup)
    {
        GroupId = workflowGroup?.Id;
        GroupName = workflowGroup?.Name;
    }
}
