using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services;
using Passingwind.Abp.ElsaModule.Workflow;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowDefinitionAppService : ElsaModuleAppService, IWorkflowDefinitionAppService
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;
        private readonly WorkflowDefinitionManager _workflowDefinitionManager;
        private readonly IWorkflowLaunchpad _workflowLaunchpad;

        public WorkflowDefinitionAppService(IWorkflowDefinitionRepository workflowDefinitionRepository, IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository, WorkflowDefinitionManager workflowDefinitionManager, IWorkflowLaunchpad workflowLaunchpad)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
            _workflowDefinitionManager = workflowDefinitionManager;
            _workflowLaunchpad = workflowLaunchpad;
        }

        public virtual async Task<WorkflowDefinitionVersionDto> CreateAsync(WorkflowDefinitionVersionCreateOrUpdateDto input)
        {
            WorkflowDefinition defintion = ObjectMapper.Map<WorkflowDefinitionCreateOrUpdateDto, WorkflowDefinition>(input.Definition);
            WorkflowDefinitionVersion version = ObjectMapper.Map<WorkflowDefinitionVersionCreateOrUpdateDto, WorkflowDefinitionVersion>(input);

            version.Definition = defintion;
            version.SetIsLatest(true);

            defintion.SetVersion(version.Version, null);

            if (input.IsPublished)
            {
                version.SetIsPublished(true);
                defintion.SetVersion(version.Version, version.Version);
            }

            await _workflowDefinitionVersionRepository.InsertAsync(version);

            return ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(version);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            await _workflowDefinitionVersionRepository.DeleteAsync(x => x.DefinitionId == id);
            await _workflowDefinitionRepository.DeleteAsync(id);
        }

        public virtual async Task DeleteVersionAsync(Guid id, int version)
        {
            await _workflowDefinitionVersionRepository.DeleteAsync(x => x.DefinitionId == id && x.Version == version);
        }

        public virtual async Task<WorkflowDefinitionVersionDto> GetAsync(Guid id)
        {
            var entity = await _workflowDefinitionVersionRepository.GetLatestAsync(id);

            var dto = ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(entity);
            return dto;
        }

        public virtual async Task<WorkflowDefinitionDto> GetDefinitionAsync(Guid id)
        {
            var entity = await _workflowDefinitionRepository.GetAsync(id);

            return ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(entity);
        }

        public virtual async Task<PagedResultDto<WorkflowDefinitionDto>> GetListAsync(WorkflowDefinitionListRequestDto input)
        {
            var count = await _workflowDefinitionRepository.CountAsync();
            var list = await _workflowDefinitionRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, "id desc");

            return new PagedResultDto<WorkflowDefinitionDto>(count, ObjectMapper.Map<List<WorkflowDefinition>, List<WorkflowDefinitionDto>>(list));
        }

        public virtual async Task<WorkflowDefinitionVersionDto> GetVersionAsync(Guid id, int version)
        {
            var entity = await _workflowDefinitionVersionRepository.GetAsync(x => x.DefinitionId == id && x.Version == version);

            var dto = ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(entity);
            return dto;
        }

        public virtual async Task<PagedResultDto<WorkflowDefinitionVersionListItemDto>> GetVersionsAsync(Guid id, WorkflowDefinitionVersionListRequestDto input)
        {
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
            int latestVersion = defintion.LatestVersion;

            if (input.IsPublished)
            {
                await _workflowDefinitionManager.UnsetPublishedVersionAsync(id);
            }

            if (defintion.PublishedVersion == defintion.LatestVersion)
            {
                await _workflowDefinitionManager.UnsetLatestVersionAsync(id);


                currentVersion = await _workflowDefinitionManager.CreateDefinitionVersionAsync(id, CurrentTenant.Id, ObjectMapper.Map<List<ActivityCreateOrUpdateDto>, List<Activity>>(input.Activities), ObjectMapper.Map<List<ActivityConnectionCreateDto>, List<ActivityConnection>>(input.Connections));

                currentVersion.SetVersion(defintion.LatestVersion + 1);
                currentVersion.SetIsLatest(true);

                if (input.IsPublished)
                {
                    currentVersion.SetIsPublished(true);
                }

                // new version
                await _workflowDefinitionVersionRepository.InsertAsync(currentVersion);

                await _workflowDefinitionVersionRepository.EnsurePropertyLoadedAsync(currentVersion, x => x.Definition);

                defintion = currentVersion.Definition;
            }
            else
            {
                currentVersion = await _workflowDefinitionVersionRepository.FindAsync(x => x.DefinitionId == id && x.Version == defintion.LatestVersion);

                if (currentVersion == null)
                    throw new UserFriendlyException($"the latest versiont '{defintion.LatestVersion}' not found.");

                currentVersion = ObjectMapper.Map<WorkflowDefinitionVersionCreateOrUpdateDto, WorkflowDefinitionVersion>(input, currentVersion);


                if (input.IsPublished)
                {
                    currentVersion.SetIsPublished(true);
                }

                // update latest version 
                await _workflowDefinitionVersionRepository.UpdateAsync(currentVersion);

                defintion = currentVersion.Definition;
            }

            if (input.IsPublished)
                defintion.SetVersion(currentVersion.Version, currentVersion.Version);
            else
                defintion.SetLatestVersion(currentVersion.Version);

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>(currentVersion);
        }

        public virtual async Task<WorkflowDefinitionDto> UpdateDefinitionAsync(Guid id, WorkflowDefinitionCreateOrUpdateDto input)
        {
            var entity = await _workflowDefinitionRepository.GetAsync(id);

            ObjectMapper.Map<WorkflowDefinitionCreateOrUpdateDto, WorkflowDefinition>(input, entity);

            await _workflowDefinitionRepository.UpdateAsync(entity);

            return ObjectMapper.Map<WorkflowDefinition, WorkflowDefinitionDto>(entity);
        }

        public virtual async Task UnPublishAsync(Guid id)
        {
            var entity = await _workflowDefinitionRepository.GetAsync(id);

            if (entity.PublishedVersion.HasValue)
            {
                var version = await _workflowDefinitionVersionRepository.GetAsync(x => x.DefinitionId == id && x.Version == entity.PublishedVersion.Value);

                version.SetIsPublished(false);

                entity = version.Definition;
                entity.SetPublishedVersion(null);

                await _workflowDefinitionVersionRepository.UpdateAsync(version);
            }
        }

        public virtual async Task PublishAsync(Guid id)
        {
            var entity = await _workflowDefinitionRepository.GetAsync(id);

            if (!entity.PublishedVersion.HasValue)
            {
                var version = await _workflowDefinitionVersionRepository.GetAsync(x => x.DefinitionId == id && x.Version == entity.LatestVersion);

                version.SetIsPublished(true);

                entity = version.Definition;
                entity.SetPublishedVersion(entity.LatestVersion);

                await _workflowDefinitionVersionRepository.UpdateAsync(version);
            }
        }

        public async Task<WorkflowDefinitionDispatchResultDto> DispatchAsync(Guid id, WorkflowDefinitionDispatchRequestDto input)
        {
            var entity = await _workflowDefinitionVersionRepository.GetLatestAsync(id);

            var tenantId = CurrentTenant.Id?.ToString();

            var startableWorkflow = await _workflowLaunchpad.FindStartableWorkflowAsync(id.ToString(), input.ActivityId?.ToString(), input.CorrelationId, input.ContextId, tenantId);

            if (startableWorkflow == null)
                throw new UserFriendlyException("The workflow is not found.");

            var result = await _workflowLaunchpad.DispatchStartableWorkflowAsync(startableWorkflow, new WorkflowInput(input.Input));

            return new WorkflowDefinitionDispatchResultDto
            {
                WorkflowInstanceId = Guid.Parse(result.WorkflowInstanceId),
            };
        }

        public async Task ExecuteAsync(Guid id, WorkflowDefinitionExecuteRequestDto input)
        {
            var entity = await _workflowDefinitionVersionRepository.GetLatestAsync(id);

            var tenantId = CurrentTenant.Id?.ToString();

            var startableWorkflow = await _workflowLaunchpad.FindStartableWorkflowAsync(id.ToString(), input.ActivityId?.ToString(), input.CorrelationId, input.ContextId, tenantId);

            if (startableWorkflow == null)
                throw new UserFriendlyException("The workflow is not found.");

            var result = await _workflowLaunchpad.ExecuteStartableWorkflowAsync(startableWorkflow, new WorkflowInput(input.Input));
        }
    }
}
