using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elsa.Models;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowDefinitionManager : DomainService
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;

    public WorkflowDefinitionManager(IWorkflowDefinitionRepository workflowDefinitionRepository, IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
    }

    public virtual async Task CheckNameExistsAsync(WorkflowDefinition entity)
    {
        var exists = await _workflowDefinitionRepository.AnyAsync(x => x.Name == entity.Name && x.Id != entity.Id);

        if (exists)
            throw new BusinessException(ElsaModuleErrorCodes.WorkflowDefinitionNameExists).WithData("name", entity.Name);
    }

    public virtual Task<WorkflowDefinition> CreateDefinitionAsync(string name, string displayName, Guid? tenantId, string description, bool isSingleton, bool deleteCompletedInstances, string channel, string tag, WorkflowPersistenceBehavior persistenceBehavior, WorkflowContextOptions contextOptions, Dictionary<string, object> variables, Dictionary<string, object> customAttributes)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        var definition = new WorkflowDefinition(GuidGenerator.Create(), name, displayName ?? name, tenantId, description, isSingleton, deleteCompletedInstances, channel, tag, persistenceBehavior, contextOptions, variables, customAttributes);

        return Task.FromResult(definition);
    }

    public virtual Task<WorkflowDefinitionVersion> CreateDefinitionVersionAsync(Guid definitionId, Guid? tenantId, List<Activity> activities, List<ActivityConnection> connections)
    {
        if (activities == null)
            throw new ArgumentNullException(nameof(activities));

        var entity = new WorkflowDefinitionVersion(definitionId, tenantId, activities, connections);

        return Task.FromResult(entity);
    }

    public virtual Task UpdateDefinitionAsync(
        WorkflowDefinition entity,
        string displayName,
        string description,
        bool isSingleton,
        bool deleteCompletedInstances,
        string channel,
        string tag,
        WorkflowPersistenceBehavior persistenceBehavior,
        WorkflowContextOptions contextOptions,
        Dictionary<string, object> variables,
        Dictionary<string, object> customAttributes)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.DisplayName = displayName;
        entity.Channel = channel;
        entity.Description = description;
        entity.IsSingleton = isSingleton;
        entity.DeleteCompletedInstances = deleteCompletedInstances;
        entity.Tag = tag;
        entity.PersistenceBehavior = persistenceBehavior;
        entity.ContextOptions = contextOptions;
        entity.Variables = variables;
        entity.CustomAttributes = customAttributes;

        return Task.CompletedTask;
    }

    public virtual Task UpdateDefinitionVersionAsync(WorkflowDefinitionVersion entity, List<Activity> activities, List<ActivityConnection> connections)
    {
        entity.Activities = activities;
        entity.Connections = connections;

        return Task.CompletedTask;
    }

    public virtual async Task UnsetLatestVersionAsync(Guid definitionId)
    {
        var entity = await _workflowDefinitionVersionRepository.FindAsync(x => x.DefinitionId == definitionId && x.IsLatest);

        if (entity != null)
        {
            entity.SetIsLatest(false);
            await _workflowDefinitionVersionRepository.UpdateAsync(entity);
        }
    }

    public virtual async Task UnsetPublishedVersionAsync(Guid definitionId)
    {
        var entity = await _workflowDefinitionVersionRepository.FindAsync(x => x.DefinitionId == definitionId && x.IsPublished);

        if (entity != null)
        {
            entity.SetIsPublished(false);
            await _workflowDefinitionVersionRepository.UpdateAsync(entity);
        }
    }
}
