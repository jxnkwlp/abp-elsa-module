﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.WorkflowDefinitions;
using Passingwind.Abp.ElsaModule.Common;
using WorkflowDefinitionModel = Elsa.Models.WorkflowDefinition;

namespace Passingwind.Abp.ElsaModule.Stores;

public class WorkflowDefinitionStore : Store<WorkflowDefinitionModel, WorkflowDefinitionVersion, Guid>, IWorkflowDefinitionStore
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;

    public WorkflowDefinitionStore(IWorkflowDefinitionRepository workflowDefinitionRepository)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
    }

    protected override Task<WorkflowDefinitionVersion> MapToEntityAsync(WorkflowDefinitionModel model)
    {
        throw new NotImplementedException();
        //return StoreMapper.MapToEntity(model);
    }

    protected override Task<WorkflowDefinitionVersion> MapToEntityAsync(WorkflowDefinitionModel model, WorkflowDefinitionVersion entity)
    {
        throw new NotImplementedException();
        // return StoreMapper.MapToEntity(model, entity);
    }

    protected override async Task<WorkflowDefinitionModel> MapToModelAsync(WorkflowDefinitionVersion entity)
    {
        var definitionId = entity.DefinitionId;
        var definition = await _workflowDefinitionRepository.GetAsync(definitionId);

        return StoreMapper.MapToModel(entity, definition);
    }

    protected override async Task<Expression<Func<WorkflowDefinitionVersion, bool>>> MapSpecificationAsync(ISpecification<WorkflowDefinitionModel> specification)
    {
        switch (specification)
        {
            case LatestOrPublishedWorkflowDefinitionIdSpecification latestOrPublishedWorkflowDefinitionIdSpecification:
                {
                    return x => x.DefinitionId == Guid.Parse(latestOrPublishedWorkflowDefinitionIdSpecification.WorkflowDefinitionId) && (x.IsLatest || x.IsPublished);
                }

            case ManyWorkflowDefinitionIdsSpecification manyWorkflowDefinitionIdsSpecification:
                {
                    var ids = manyWorkflowDefinitionIdsSpecification.Ids.ToList().ConvertAll(Guid.Parse);

                    Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => ids.Contains(x.DefinitionId);

                    return expression.WithVersion(manyWorkflowDefinitionIdsSpecification.VersionOptions);
                }

            case ManyWorkflowDefinitionNamesSpecification manyWorkflowDefinitionNamesSpecification:
                {
                    var ids = await _workflowDefinitionRepository.GetIdsByNamesAsync(manyWorkflowDefinitionNamesSpecification.Names);

                    return x => ids.Contains(x.Id);
                }

            case ManyWorkflowDefinitionVersionIdsSpecification manyWorkflowDefinitionVersionIdsSpecification:
                {
                    var ids = manyWorkflowDefinitionVersionIdsSpecification.DefinitionVersionIds.ToList().ConvertAll(Guid.Parse);
                    return x => ids.Contains(x.Id);
                }

            case VersionOptionsSpecification versionOptionsSpecification:
                {
                    Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => true;

                    return expression.WithVersion(versionOptionsSpecification.VersionOptions);
                }

            case WorkflowDefinitionIdSpecification workflowDefinitionIdSpecification:
                {
                    var tenantId = workflowDefinitionIdSpecification.TenantId.ToGuid();

                    Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => x.DefinitionId == Guid.Parse(workflowDefinitionIdSpecification.Id) && x.TenantId == tenantId;

                    return expression.WithVersion(workflowDefinitionIdSpecification.VersionOptions);
                }

            case WorkflowDefinitionNameSpecification workflowDefinitionNameSpecification:
                {
                    var tenantId = workflowDefinitionNameSpecification.TenantId.ToGuid();
                    var id = await _workflowDefinitionRepository.GetIdByNameAsync(workflowDefinitionNameSpecification.Name);

                    Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => x.Id == id && x.TenantId == tenantId;

                    return expression.WithVersion(workflowDefinitionNameSpecification.VersionOptions);
                }

            case WorkflowDefinitionSearchTermSpecification workflowDefinitionSearchTermSpecification:
                {
                    var filter = workflowDefinitionSearchTermSpecification.SearchTerm;

                    if (string.IsNullOrEmpty(filter))
                        return x => true;

                    var list = await _workflowDefinitionRepository.GetListAsync(filter);
                    var ids = list.Select(x => x.Id).ToArray();
                    return x => ids.Contains(x.Id);
                }

            case WorkflowDefinitionTagSpecification workflowDefinitionTagSpecification:
                {
                    var tenantId = workflowDefinitionTagSpecification.TenantId.ToGuid();

                    var id = await _workflowDefinitionRepository.GetIdByTagAsync(workflowDefinitionTagSpecification.Tag);

                    Expression<Func<WorkflowDefinitionVersion, bool>> expression = x => x.Id == id && x.TenantId == tenantId;

                    return expression.WithVersion(workflowDefinitionTagSpecification.VersionOptions);
                }

            case WorkflowDefinitionVersionIdSpecification workflowDefinitionVersionIdSpecification:
                {
                    return x => x.Id == Guid.Parse(workflowDefinitionVersionIdSpecification.VersionId);
                }

            default:
                return await base.MapSpecificationAsync(specification);
        }
    }

    protected override Guid ConvertKey(string value)
    {
        return Guid.Parse(value);
    }
}
