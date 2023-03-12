using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public interface IWorkflowDefinitionAppService : ICrudAppService<WorkflowDefinitionVersionDto, WorkflowDefinitionDto, Guid, WorkflowDefinitionListRequestDto, WorkflowDefinitionVersionCreateOrUpdateDto, WorkflowDefinitionVersionCreateOrUpdateDto>
{
    Task<PagedResultDto<WorkflowDefinitionVersionListItemDto>> GetVersionsAsync(Guid id, WorkflowDefinitionVersionListRequestDto input);
    Task DeleteVersionAsync(Guid id, int version);
    Task<WorkflowDefinitionVersionDto> GetVersionAsync(Guid id, int version);

    Task<WorkflowDefinitionDto> GetDefinitionAsync(Guid id);
    Task<WorkflowDefinitionDto> UpdateDefinitionAsync(Guid id, WorkflowDefinitionCreateOrUpdateDto input);

    Task UnPublishAsync(Guid id);
    Task PublishAsync(Guid id);

    Task ExecuteAsync(Guid id, WorkflowDefinitionExecuteRequestDto input);
    Task<WorkflowDefinitionDispatchResultDto> DispatchAsync(Guid id, WorkflowDefinitionDispatchRequestDto input);

    Task<WorkflowDefinitionVersionDto> GetPreviousVersionAsync(Guid id, int version);

    Task RetractAsync(Guid id);
    Task RevertAsync(Guid id, WorkflowDefinitionRevertRequestDto input);

    Task<WorkflowDefinitionIamResultDto> GetIamAsync(Guid id);

    Task AddOwnerAsync(Guid id, WorkflowDefinitionAddOwnerRequestDto input);
    Task DeleteOwnerAsync(Guid id, Guid userId);
}
