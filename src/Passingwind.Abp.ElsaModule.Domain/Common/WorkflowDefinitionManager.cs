using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elsa.Models;
using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowDefinitionManager : DomainService
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;

        public WorkflowDefinitionManager(IWorkflowDefinitionRepository workflowDefinitionRepository, IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
        }

        public virtual Task<WorkflowDefinition> CreateDefinitionAsync(string name, string displayName, Guid? tenantId, string description, bool isSingleton, bool deleteCompletedInstances, string channel, string tag, WorkflowPersistenceBehavior persistenceBehavior, WorkflowContextOptions contextOptions, Dictionary<string, object> variables, Dictionary<string, object> customAttributes)
        {
            var definition = new WorkflowDefinition(name, displayName, tenantId, description, isSingleton, deleteCompletedInstances, channel, tag, persistenceBehavior, contextOptions, variables, customAttributes);

            return Task.FromResult(definition);
        }

        public virtual Task<WorkflowDefinitionVersion> CreateDefinitionVersionAsync(Guid definitionId, Guid? tenantId, List<Activity> activities, List<ActivityConnection> connections)
        {
            var entity = new WorkflowDefinitionVersion(definitionId, tenantId, activities, connections);

            return Task.FromResult(entity);
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
}
