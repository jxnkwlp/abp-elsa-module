using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Permissions;
using Passingwind.Abp.ElsaModule.Teams;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.Identity;
using Volo.Abp.Json;

namespace Passingwind.Abp.ElsaModule.Common;

[Authorize(Policy = ElsaModulePermissions.Definitions.Default)]
public class WorkflowDefinitionAppService : ElsaModuleAppService, IWorkflowDefinitionAppService
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;
    private readonly WorkflowDefinitionManager _workflowDefinitionManager;
    private readonly IWorkflowLaunchpad _workflowLaunchpad;
    private readonly IWorkflowPermissionProvider _workflowPermissionProvider;
    private readonly IWorkflowTeamManager _workflowTeamManager;
    private readonly IdentityUserManager _identityUserManager;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IWorkflowImporter _workflowImporter;

    public WorkflowDefinitionAppService(
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository,
        WorkflowDefinitionManager workflowDefinitionManager,
        IWorkflowLaunchpad workflowLaunchpad,
        IWorkflowPermissionProvider workflowPermissionProvider,
        IWorkflowTeamManager workflowTeamManager,
        IdentityUserManager identityUserManager,
        IJsonSerializer jsonSerializer,
        IWorkflowImporter workflowImporter)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
        _workflowDefinitionManager = workflowDefinitionManager;
        _workflowLaunchpad = workflowLaunchpad;
        _workflowPermissionProvider = workflowPermissionProvider;
        _workflowTeamManager = workflowTeamManager;
        _identityUserManager = identityUserManager;
        _jsonSerializer = jsonSerializer;
        _workflowImporter = workflowImporter;
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish)]
    public virtual async Task<WorkflowDefinitionVersionDto> CreateAsync(WorkflowDefinitionVersionCreateOrUpdateDto input)
    {
        WorkflowDefinition defintion = await _workflowDefinitionManager.CreateDefinitionAsync(
            input.Definition.Name,
            input.Definition.DisplayName,
            CurrentTenant.Id,
            input.Definition.Description,
            input.Definition.IsSingleton,
            input.Definition.DeleteCompletedInstances,
            input.Definition.Channel,
            input.Definition.Tag,
            input.Definition.PersistenceBehavior,
            input.Definition.ContextOptions,
            input.Definition.Variables,
            default
        );
        defintion.SetVersion(1, null);

        // check name
        await _workflowDefinitionManager.CheckNameExistsAsync(defintion);

        WorkflowDefinitionVersion version = await _workflowDefinitionManager.CreateDefinitionVersionAsync(
            defintion.Id,
            CurrentTenant.Id,
            input.Activities?.Select(x => new Activity(
                x.ActivityId,
                x.Type,
                x.Name,
                x.DisplayName,
                x.Description,
                x.PersistWorkflow,
                x.LoadWorkflowContext,
                x.SaveWorkflowContext,
                x.Attributes,
                x.Properties,
                x.PropertyStorageProviders))?.ToList(),
            input.Connections.Select(x => new ActivityConnection(
                x.SourceId,
                x.TargetId,
                x.Outcome,
                x.Attributes))?.ToList()
            );

        version.SetIsLatest(true);

        if (input.IsPublished)
        {
            version.SetIsPublished(true);
            defintion.SetVersion(version.Version, version.Version);
        }

        defintion = await _workflowDefinitionRepository.InsertAsync(defintion);
        version = await _workflowDefinitionVersionRepository.InsertAsync(version);

        var dto = ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(version);
        dto.Definition = ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(defintion);
        return dto;
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await CheckWorkflowPermissionAsync(id, ElsaModulePermissions.Definitions.Delete);

        await _workflowDefinitionVersionRepository.DeleteAsync(x => x.DefinitionId == id);
        await _workflowDefinitionRepository.DeleteAsync(id);
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.Delete)]
    public virtual async Task DeleteVersionAsync(Guid id, int version)
    {
        await CheckWorkflowPermissionAsync(id, ElsaModulePermissions.Definitions.Delete);

        await _workflowDefinitionVersionRepository.DeleteAsync(x => x.DefinitionId == id && x.Version == version);
    }

    public virtual async Task<WorkflowDefinitionVersionDto> GetAsync(Guid id)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.Default);

        var dto = ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(version);
        dto.Definition = ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(entity);

        return dto;
    }

    public virtual async Task<WorkflowDefinitionDto> GetDefinitionAsync(Guid id)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.Default);

        return ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(entity);
    }

    public virtual async Task<PagedResultDto<WorkflowDefinitionBasicDto>> GetListAsync(WorkflowDefinitionListRequestDto input)
    {
        var grantedResult = await _workflowPermissionProvider.GetGrantsAsync();

        var count = await _workflowDefinitionRepository.CountAsync(
            name: input.Filter,
            isSingleton: input.IsSingleton,
            filterIds: grantedResult.AllGranted ? null : grantedResult.WorkflowIds);
        var list = await _workflowDefinitionRepository.GetPagedListAsync(
            input.SkipCount,
            input.MaxResultCount,
            name: input.Filter,
            isSingleton: input.IsSingleton,
            filterIds: grantedResult.AllGranted ? null : grantedResult.WorkflowIds,
            ordering: input.Sorting);

        return new PagedResultDto<WorkflowDefinitionBasicDto>(count, ObjectMapper.Map<List<WorkflowDefinition>, List<WorkflowDefinitionBasicDto>>(list));
    }

    public virtual async Task<WorkflowDefinitionVersionDto> GetVersionAsync(Guid id, int version)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);
        var versionEntity = await _workflowDefinitionVersionRepository.GetByVersionAsync(id, version);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.Default);

        var dto = ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(versionEntity);
        dto.Definition = ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(entity);
        return dto;
    }

    public virtual async Task<WorkflowDefinitionVersionDto> GetPreviousVersionAsync(Guid id, int version)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);
        var previousVersion = await _workflowDefinitionVersionRepository.GetPreviousVersionNumberAsync(id, version);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.Default);

        if (previousVersion.HasValue == false)
            return null;

        var versionEntity = await _workflowDefinitionVersionRepository.GetByVersionAsync(id, previousVersion.Value);

        var dto = ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(versionEntity);
        dto.Definition = ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(entity);
        return dto;
    }

    public virtual async Task<PagedResultDto<WorkflowDefinitionVersionListItemDto>> GetVersionsAsync(Guid id, WorkflowDefinitionVersionListRequestDto input)
    {
        await CheckWorkflowPermissionAsync(id, ElsaModulePermissions.Definitions.Default);

        var count = await _workflowDefinitionVersionRepository.GetCountAsync(x => x.DefinitionId == id);
        var list = await _workflowDefinitionVersionRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, x => x.DefinitionId == id, "version desc", false);

        return new PagedResultDto<WorkflowDefinitionVersionListItemDto>(count, ObjectMapper.Map<List<WorkflowDefinitionVersion>, List<WorkflowDefinitionVersionListItemDto>>(list));
    }

    public virtual async Task<WorkflowDefinitionVersionDto> UpdateAsync(Guid id, WorkflowDefinitionVersionCreateOrUpdateDto input)
    {
        // if publish version eq latest version, then create next version record
        // else update latest version

        WorkflowDefinitionVersion currentVersion = null;
        WorkflowDefinition defintion = await _workflowDefinitionRepository.GetAsync(id);

        await AuthorizationService.CheckAsync(defintion, ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish);

        int latestVersion = defintion.LatestVersion;

        if (input.IsPublished)
            await _workflowDefinitionManager.UnsetPublishedVersionAsync(id);

        var activities = input.Activities?.Select(x => new Activity(
                   x.ActivityId,
                   x.Type,
                   x.Name,
                   x.DisplayName,
                   x.Description,
                   x.PersistWorkflow,
                   x.LoadWorkflowContext,
                   x.SaveWorkflowContext,
                   x.Attributes,
                   x.Properties,
                   x.PropertyStorageProviders))?.ToList();

        var connections = input.Connections.Select(x => new ActivityConnection(
                x.SourceId,
                x.TargetId,
                x.Outcome,
                x.Attributes))?.ToList();

        // check version
        if (defintion.PublishedVersion == defintion.LatestVersion)
        {
            await _workflowDefinitionManager.UnsetLatestVersionAsync(id);

            currentVersion = await _workflowDefinitionManager.CreateDefinitionVersionAsync(
                id,
                CurrentTenant.Id,
                activities,
                connections);

            currentVersion.SetVersion(defintion.LatestVersion + 1);
            currentVersion.SetIsLatest(true);

            if (input.IsPublished)
                currentVersion.SetIsPublished(true);

            // new version
            await _workflowDefinitionVersionRepository.InsertAsync(currentVersion);

        }
        else
        {
            currentVersion = await _workflowDefinitionVersionRepository.FindAsync(x => x.DefinitionId == id && x.Version == defintion.LatestVersion);

            if (currentVersion == null)
                throw new UserFriendlyException($"The latest versiont '{defintion.LatestVersion}' not found.");

            await _workflowDefinitionManager.UpdateDefinitionVersionAsync(
                currentVersion,
                activities,
                connections);

            if (input.IsPublished)
                currentVersion.SetIsPublished(true);

            // update latest version 
            await _workflowDefinitionVersionRepository.UpdateAsync(currentVersion);
        }

        // check version
        if (input.IsPublished)
            defintion.SetVersion(currentVersion.Version, currentVersion.Version);
        else
            defintion.SetLatestVersion(currentVersion.Version);

        // update  
        defintion.ChangeName(input.Definition.Name);
        await _workflowDefinitionManager.UpdateDefinitionAsync(
            defintion,
            input.Definition.DisplayName,
            input.Definition.Description,
            input.Definition.IsSingleton,
            input.Definition.DeleteCompletedInstances,
            input.Definition.Channel,
            input.Definition.Tag,
            input.Definition.PersistenceBehavior,
            input.Definition.ContextOptions,
            input.Definition.Variables,
            defintion.CustomAttributes);

        // check name
        await _workflowDefinitionManager.CheckNameExistsAsync(defintion);

        // update
        await _workflowDefinitionRepository.UpdateAsync(defintion);

        var dto = ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(currentVersion);
        dto.Definition = ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(defintion);
        return dto;
    }

    public virtual async Task<WorkflowDefinitionDto> UpdateDefinitionAsync(Guid id, WorkflowDefinitionCreateOrUpdateDto input)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish);

        entity.ChangeName(input.Name);
        await _workflowDefinitionManager.UpdateDefinitionAsync(
               entity,
               input.DisplayName,
               input.Description,
               input.IsSingleton,
               input.DeleteCompletedInstances,
               input.Channel,
               input.Tag,
               input.PersistenceBehavior,
               input.ContextOptions,
               input.Variables,
               entity.CustomAttributes);

        // check name
        await _workflowDefinitionManager.CheckNameExistsAsync(entity);

        // update
        await _workflowDefinitionRepository.UpdateAsync(entity);

        return ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(entity);
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish)]
    public virtual async Task UnPublishAsync(Guid id)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish);

        if (entity.PublishedVersion.HasValue)
        {
            var version = await _workflowDefinitionVersionRepository.GetByVersionAsync(id, entity.PublishedVersion.Value);

            // update version
            version.SetIsPublished(false);
            await _workflowDefinitionVersionRepository.UpdateAsync(version);

            // update definition
            entity.SetPublishedVersion(null);
            await _workflowDefinitionRepository.UpdateAsync(entity);
        }
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish)]
    public virtual async Task PublishAsync(Guid id)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish);

        if (!entity.PublishedVersion.HasValue)
        {
            var version = await _workflowDefinitionVersionRepository.GetByVersionAsync(id, entity.LatestVersion);

            // update version
            version.SetIsPublished(true);
            await _workflowDefinitionVersionRepository.UpdateAsync(version);

            // update definition
            entity.SetPublishedVersion(entity.LatestVersion);
            await _workflowDefinitionRepository.UpdateAsync(entity);
        }
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.Dispatch)]
    public async Task<WorkflowDefinitionDispatchResultDto> DispatchAsync(Guid id, WorkflowDefinitionDispatchRequestDto input)
    {
        var entity = await _workflowDefinitionVersionRepository.GetLatestAsync(id);

        await CheckWorkflowPermissionAsync(entity.DefinitionId, ElsaModulePermissions.Definitions.Dispatch);

        var tenantId = CurrentTenant.Id?.ToString();

        var startableWorkflow = await _workflowLaunchpad.FindStartableWorkflowAsync(id.ToString(), input.ActivityId?.ToString(), input.CorrelationId, input.ContextId, tenantId);

        if (startableWorkflow == null)
            throw new UserFriendlyException("Cannot start workflow. Maybe it has no starting activities.");

        var result = await _workflowLaunchpad.DispatchStartableWorkflowAsync(startableWorkflow, new WorkflowInput(input.Input));

        return new WorkflowDefinitionDispatchResultDto
        {
            WorkflowInstanceId = Guid.Parse(result.WorkflowInstanceId),
        };
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.Dispatch)]
    public async Task ExecuteAsync(Guid id, WorkflowDefinitionExecuteRequestDto input)
    {
        var entity = await _workflowDefinitionVersionRepository.GetLatestAsync(id);

        await CheckWorkflowPermissionAsync(entity.DefinitionId, ElsaModulePermissions.Definitions.Dispatch);

        var tenantId = CurrentTenant.Id?.ToString();

        var startableWorkflow = await _workflowLaunchpad.FindStartableWorkflowAsync(id.ToString(), input.ActivityId?.ToString(), input.CorrelationId, input.ContextId, tenantId);

        if (startableWorkflow == null)
            throw new UserFriendlyException("Cannot start workflow. Maybe it has no starting activities.");

        var result = await _workflowLaunchpad.ExecuteStartableWorkflowAsync(startableWorkflow, new WorkflowInput(input.Input));
    }

    public Task RetractAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task RevertAsync(Guid id, WorkflowDefinitionRevertRequestDto input)
    {
        throw new NotImplementedException();
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.ManagePermissions)]
    public async Task<WorkflowDefinitionIamResultDto> GetIamAsync(Guid id)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);
        var groups = await _workflowTeamManager.GetListByWorkflowIdAsync(id);

        var owners = await _workflowPermissionProvider.GetWorkflowOwnersAsync(entity);

        return new WorkflowDefinitionIamResultDto
        {
            Teams = ObjectMapper.Map<IEnumerable<WorkflowTeam>, IEnumerable<WorkflowTeamBasicDto>>(groups),
            Owners = ObjectMapper.Map<IEnumerable<IdentityUser>, IEnumerable<IdentityUserDto>>(owners),
        };
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.ManagePermissions)]
    public async Task AddOwnerAsync(Guid id, WorkflowDefinitionAddOwnerRequestDto input)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);
        var user = await _identityUserManager.GetByIdAsync(input.UserId);

        await _workflowPermissionProvider.AddWorkflowOwnerAsync(entity, user);
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.ManagePermissions)]
    public async Task DeleteOwnerAsync(Guid id, Guid userId)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);
        var user = await _identityUserManager.GetByIdAsync(userId);

        await _workflowPermissionProvider.RemoveWorkflowOwnerAsync(entity, user);
    }

    public async Task<WorkflowVariablesDto> GetVariablesAsync(Guid id)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.Default);

        return new WorkflowVariablesDto
        {
            Variables = entity.Variables,
        };
    }

    public async Task<WorkflowVariablesDto> UpdateVariablesAsync(Guid id, WorkflowVariableUpdateDto input)
    {
        var entity = await _workflowDefinitionRepository.GetAsync(id);

        await CheckWorkflowPermissionAsync(entity, ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish);

        entity.Variables = input.Variables;

        await _workflowDefinitionRepository.UpdateAsync(entity);

        return new WorkflowVariablesDto
        {
            Variables = entity.Variables,
        };
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.Export)]
    public async Task<IRemoteStreamContent> ExportAsync(WorkflowDefinitionExportRequestDto input)
    {
        var filterIds = await FilterWorkflowsAsync(input.Ids);

        var list = await _workflowDefinitionRepository.GetListAsync(filterIds: filterIds);

        var versionList = await _workflowDefinitionVersionRepository.GetListAsync(definitionIds: filterIds, isPublished: true, includeDetails: true);

        var randomName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
        var zipFilePath = Path.Combine(Path.GetTempPath(), "workflow", "export", $"{randomName}.zip");
        // create directory
        var exportTempDir = Path.Combine(Path.GetTempPath(), "workflow", "export", randomName);
        if (!Directory.Exists(exportTempDir))
            Directory.CreateDirectory(exportTempDir);

        var jsonNodeOptions = new JsonNodeOptions() { PropertyNameCaseInsensitive = true };

        try
        {
            // write file 
            foreach (var item in list)
            {
                var version = versionList.FirstOrDefault(x => x.DefinitionId == item.Id);

                var content = _jsonSerializer.Serialize(item);

                if (version != null)
                {
                    var jsonObject = JsonObject.Parse(content, jsonNodeOptions);
                    jsonObject["activities"] = JsonNode.Parse(_jsonSerializer.Serialize(version.Activities), jsonNodeOptions);
                    jsonObject["connections"] = JsonNode.Parse(_jsonSerializer.Serialize(version.Connections), jsonNodeOptions);

                    content = jsonObject.ToJsonString();
                }

                var itemFile = Path.Combine(exportTempDir, item.Name + ".json");
                File.WriteAllText(itemFile, content);
            }

            ZipFile.CreateFromDirectory(exportTempDir, zipFilePath);

            Logger.LogInformation("Workflow definition export zip file created: {0}", zipFilePath);

            return new RemoteStreamContent(File.OpenRead(zipFilePath), $"{DateTime.UtcNow:yyyyMMddHHmmss}.zip", "application/stream");
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("Export workflow definition failed.", innerException: ex);
        }
    }

    [Authorize(Policy = ElsaModulePermissions.Definitions.Import)]
    public async Task<WorkflowDefinitionImportResultDto> ImportAsync(WorkflowDefinitionImportRequestDto input)
    {
        if (input.File?.ContentLength == 0)
            throw new ArgumentNullException();

        var randomName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
        var zipFilePath = Path.Combine(Path.GetTempPath(), "workflow", "import", $"{randomName}.zip");
        var tempFileDir = Path.Combine(Path.GetTempPath(), "workflow", "import", randomName);
        if (!Directory.Exists(Path.GetDirectoryName(zipFilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath));

        bool isZip = false;

        if (Path.GetExtension(input.File.FileName) == ".zip" || input.File.ContentType == "application/zip")
            isZip = true;

        var importList = new Dictionary<string, string>();

        if (isZip)
        {
            // extract zip file
            try
            {
                using MemoryStream ms = new MemoryStream();

                await input.File.GetStream().CopyToAsync(ms);

                await File.WriteAllBytesAsync(zipFilePath, ms.ToArray());

                ZipFile.ExtractToDirectory(zipFilePath, tempFileDir, true);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Unzip file failed.", innerException: ex);
            }

            // read files
            foreach (var item in Directory.EnumerateFiles(tempFileDir))
            {
                importList[Path.GetFileName(item)] = File.ReadAllText(item);
            }
        }
        else
        {
            try
            {
                using StreamReader sr = new StreamReader(input.File.GetStream());
                var fileContent = await sr.ReadToEndAsync();

                importList[input.File.FileName] = fileContent;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Read upload file content failed.", innerException: ex);
            }
        }

        var result = new WorkflowDefinitionImportResultDto()
        {
            TotalCount = importList.Count,
        };

        foreach (var item in importList)
        {
            try
            {
                var workflow = await _workflowImporter.ImportAsync(item.Value, input.Overwrite);

                result.Add(item.Key, ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionBasicDto>(workflow));

                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Add(item.Key, ex.Message);
            }
        }

        return result;
    }
}
