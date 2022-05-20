using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.WorkflowDefinitions;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using WorkflowDefinitionModel = Elsa.Models.WorkflowDefinition;

namespace Passingwind.Abp.ElsaModule.Stores
{
    public class WorkflowDefinitionStore : Store<WorkflowDefinitionModel, WorkflowDefinitionVersion, Guid>, IWorkflowDefinitionStore
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IStoreMapper _storeMapper;

        public WorkflowDefinitionStore(ILoggerFactory loggerFactory, IRepository<WorkflowDefinitionVersion, Guid> repository, IAsyncQueryableExecuter asyncQueryableExecuter, IWorkflowDefinitionRepository workflowDefinitionRepository, IStoreMapper storeMapper) : base(loggerFactory, repository, asyncQueryableExecuter)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _storeMapper = storeMapper;
        }

        protected override Task<WorkflowDefinitionVersion> MapToEntityAsync(WorkflowDefinitionModel model)
        {
            throw new NotImplementedException();
            //return _storeMapper.MapToEntity(model);
        }

        protected override Task<WorkflowDefinitionVersion> MapToEntityAsync(WorkflowDefinitionModel model, WorkflowDefinitionVersion entity)
        {
            throw new NotImplementedException();
            // return _storeMapper.MapToEntity(model, entity);
        }

        protected override async Task<WorkflowDefinitionModel> MapToModelAsync(WorkflowDefinitionVersion entity)
        {
            var definitionId = entity.DefinitionId;
            var definition = await _workflowDefinitionRepository.GetAsync(definitionId);

            return _storeMapper.MapToModel(entity, definition);
        }

        protected override async Task<Expression<Func<WorkflowDefinitionVersion, bool>>> MapSpecificationAsync(ISpecification<WorkflowDefinitionModel> specification)
        {
            if (specification is LatestOrPublishedWorkflowDefinitionIdSpecification latestOrPublishedWorkflowDefinitionIdSpecification)
            {
                return x => x.DefinitionId == Guid.Parse(latestOrPublishedWorkflowDefinitionIdSpecification.WorkflowDefinitionId) && (x.IsLatest || x.IsPublished);
            }
            else if (specification is ManyWorkflowDefinitionIdsSpecification manyWorkflowDefinitionIdsSpecification)
            {
                var ids = manyWorkflowDefinitionIdsSpecification.Ids.ToList().ConvertAll(Guid.Parse);

                Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => ids.Contains(x.DefinitionId);

                return expression.WithVersion(manyWorkflowDefinitionIdsSpecification.VersionOptions);
            }
            else if (specification is ManyWorkflowDefinitionNamesSpecification manyWorkflowDefinitionNamesSpecification)
            {
                var ids = await _workflowDefinitionRepository.GetIdsByNamesAsync(manyWorkflowDefinitionNamesSpecification.Names);

                return x => ids.Contains(x.Id);
            }
            else if (specification is ManyWorkflowDefinitionVersionIdsSpecification manyWorkflowDefinitionVersionIdsSpecification)
            {
                var ids = manyWorkflowDefinitionVersionIdsSpecification.DefinitionVersionIds.ToList().ConvertAll(Guid.Parse);
                return x => ids.Contains(x.Id);
            }
            else if (specification is VersionOptionsSpecification versionOptionsSpecification)
            {
                Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => true;

                return expression.WithVersion(versionOptionsSpecification.VersionOptions);
            }
            else if (specification is WorkflowDefinitionIdSpecification workflowDefinitionIdSpecification)
            {
                var tenantId = workflowDefinitionIdSpecification.TenantId.ToGuid();

                Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => x.DefinitionId == Guid.Parse(workflowDefinitionIdSpecification.Id) && x.TenantId == tenantId;

                return expression.WithVersion(workflowDefinitionIdSpecification.VersionOptions);
            }
            else if (specification is WorkflowDefinitionNameSpecification workflowDefinitionNameSpecification)
            {
                var tenantId = workflowDefinitionNameSpecification.TenantId.ToGuid();
                var id = await _workflowDefinitionRepository.GetIdByNameAsync(workflowDefinitionNameSpecification.Name);

                Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => x.Id == id && x.TenantId == tenantId;

                return expression.WithVersion(workflowDefinitionNameSpecification.VersionOptions);
            }
            else if (specification is WorkflowDefinitionTagSpecification workflowDefinitionTagSpecification)
            {
                var tenantId = workflowDefinitionTagSpecification.TenantId.ToGuid();

                var id = await _workflowDefinitionRepository.GetIdByTagAsync(workflowDefinitionTagSpecification.Tag);

                Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => x.Id == id && x.TenantId == tenantId;

                return expression.WithVersion(workflowDefinitionTagSpecification.VersionOptions);
            }
            else if (specification is WorkflowDefinitionVersionIdSpecification workflowDefinitionVersionIdSpecification)
            {
                return x => x.Id == Guid.Parse(workflowDefinitionVersionIdSpecification.VersionId);
            }
            else
                return await base.MapSpecificationAsync(specification);
        }

        protected override Guid ConvertKey(string value)
        {
            return Guid.Parse(value);
        }
    }
}
