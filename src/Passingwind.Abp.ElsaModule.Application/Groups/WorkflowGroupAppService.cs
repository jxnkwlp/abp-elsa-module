using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

namespace Passingwind.Abp.ElsaModule.Groups;

public class WorkflowGroupAppService : ElsaModuleAppService, IWorkflowGroupAppService
{
    private readonly WorkflowGroupManager _workflowGroupManager;
    private readonly IWorkflowGroupRepository _workflowGroupRepository;

    public WorkflowGroupAppService(WorkflowGroupManager workflowGroupManager, IWorkflowGroupRepository workflowGroupRepository)
    {
        _workflowGroupManager = workflowGroupManager;
        _workflowGroupRepository = workflowGroupRepository;
    }

    public virtual async Task<PagedResultDto<WorkflowGroupDto>> GetListAsync(WorkflowGroupListRequestDto input)
    {
        var count = await _workflowGroupRepository.GetCountAsync(filter: input.Filter);
        var list = await _workflowGroupRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, filter: input.Filter, sorting: nameof(WorkflowGroup.CreationTime) + " desc");

        return new PagedResultDto<WorkflowGroupDto>()
        {
            Items = ObjectMapper.Map<List<WorkflowGroup>, List<WorkflowGroupDto>>(list),
            TotalCount = count,
        };
    }

    public virtual async Task<WorkflowGroupDto> GetAsync(Guid id)
    {
        var entity = await _workflowGroupRepository.GetAsync(id);

        return ObjectMapper.Map<WorkflowGroup, WorkflowGroupDto>(entity);
    }

    public virtual async Task<WorkflowGroupDto> CreateAsync(WorkflowGroupCreateOrUpdateDto input)
    {
        var entity = new WorkflowGroup(GuidGenerator.Create(), input.Name, tenantId: CurrentTenant.Id)
        {
            Description = input.Description,
        };

        input.MapExtraPropertiesTo(entity);

        await _workflowGroupRepository.InsertAsync(entity);

        return ObjectMapper.Map<WorkflowGroup, WorkflowGroupDto>(entity);
    }

    public virtual async Task<WorkflowGroupDto> UpdateAsync(Guid id, WorkflowGroupCreateOrUpdateDto input)
    {
        var entity = await _workflowGroupRepository.GetAsync(id);

        entity.SetName(input.Name);
        entity.Description = input.Description;

        input.MapExtraPropertiesTo(entity);

        await _workflowGroupRepository.UpdateAsync(entity);

        return ObjectMapper.Map<WorkflowGroup, WorkflowGroupDto>(entity);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _workflowGroupRepository.DeleteAsync(id);
    }
}
