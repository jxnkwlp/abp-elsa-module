using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Permissions;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowImporter : IWorkflowImporter
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly WorkflowDefinitionManager _workflowDefinitionManager;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;
    private readonly IWorkflowPermissionProvider _workflowPermissionService;

    public WorkflowImporter(
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        WorkflowDefinitionManager workflowDefinitionManager,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository,
        IWorkflowPermissionProvider workflowPermissionService)
    {
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _workflowDefinitionManager = workflowDefinitionManager;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
        _workflowPermissionService = workflowPermissionService;
    }

    public async Task<WorkflowDefinition> ImportAsync(string jsonContent, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        var jsonNode = JsonNode.Parse(jsonContent, new JsonNodeOptions() { PropertyNameCaseInsensitive = true });

        var name = jsonNode["Name"].GetValue<string>();
        var version = jsonNode["PublishedVersion"].GetValue<int>();

        var workflow = await _workflowDefinitionRepository.FindAsync(x => x.Name == name);

        // check exists workflow
        if (workflow != null)
        {
            // check permssion
            if (!await _workflowPermissionService.IsGrantedAsync(workflow.Id, ElsaModulePermissions.Definitions.Import))
            {
                throw new Exception($"Workflow '{name}' unauthorized access.");
            }

            if (!overwrite)
            {
                throw new Exception($"Workflow '{name}' already exists.");
            }

            if (workflow.PublishedVersion > version)
            {
                throw new Exception($"Workflow '{name}' import version '{version}' less then current version '{workflow.PublishedVersion}'.");
            }
        }

        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        // definition
        var inputWorkflow = jsonNode.Deserialize<WorkflowDefinition>(jsonSerializerOptions);

        // version
        var activities = jsonNode["activities"].Deserialize<List<Activity>>(jsonSerializerOptions);
        var connections = jsonNode["connections"].Deserialize<List<ActivityConnection>>(jsonSerializerOptions);

        // workflow definition 
        if (workflow == null)
        {
            workflow = await _workflowDefinitionManager.CreateDefinitionAsync(
                name,
                inputWorkflow.DisplayName,
                _currentTenant.Id,
                inputWorkflow.Description,
                inputWorkflow.IsSingleton,
                inputWorkflow.DeleteCompletedInstances,
                inputWorkflow.Channel,
                inputWorkflow.Tag,
                inputWorkflow.PersistenceBehavior,
                inputWorkflow.ContextOptions,
                inputWorkflow.Variables,
                inputWorkflow.CustomAttributes
            );

            workflow.SetLatestVersion(version);
            workflow.SetPublishedVersion(version);

            workflow = await _workflowDefinitionRepository.InsertAsync(workflow);

            // add permission
            // auto added by event handler
        }
        else
        {
            // update
            await _workflowDefinitionManager.UpdateDefinitionAsync(
                workflow,
                inputWorkflow.DisplayName,
                inputWorkflow.Description,
                inputWorkflow.IsSingleton,
                inputWorkflow.DeleteCompletedInstances,
                inputWorkflow.Channel,
                inputWorkflow.Tag,
                inputWorkflow.PersistenceBehavior,
                inputWorkflow.ContextOptions,
                inputWorkflow.Variables,
                inputWorkflow.CustomAttributes);

            workflow.SetLatestVersion(version);
            workflow.SetPublishedVersion(version);

            workflow = await _workflowDefinitionRepository.UpdateAsync(workflow);
        }

        // version  
        var publishedVersion = await _workflowDefinitionVersionRepository.FindByVersionAsync(workflow.Id, version, true);

        if (publishedVersion == null)
        {
            // unset old version
            await _workflowDefinitionManager.UnsetPublishedVersionAsync(workflow.Id);
            await _workflowDefinitionManager.UnsetLatestVersionAsync(workflow.Id);

            // create new version
            var workflowVersion = await _workflowDefinitionManager.CreateDefinitionVersionAsync(workflow.Id, workflow.TenantId, activities, connections);

            workflowVersion.SetVersion(version);
            workflowVersion.SetIsLatest(true);
            workflowVersion.SetIsPublished(true);

            workflowVersion = await _workflowDefinitionVersionRepository.InsertAsync(workflowVersion);
        }
        else
        {
            // unset old version
            if (!publishedVersion.IsLatest)
                await _workflowDefinitionManager.UnsetLatestVersionAsync(workflow.Id);

            if (!publishedVersion.IsPublished)
                await _workflowDefinitionManager.UnsetPublishedVersionAsync(workflow.Id);

            publishedVersion.SetIsLatest(true);
            publishedVersion.SetIsPublished(true);

            await _workflowDefinitionManager.UpdateDefinitionVersionAsync(publishedVersion, activities, connections);
        }

        return workflow;
    }
}
