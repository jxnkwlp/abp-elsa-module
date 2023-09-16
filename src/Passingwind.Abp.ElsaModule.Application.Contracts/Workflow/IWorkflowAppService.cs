using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.Workflow;

public interface IWorkflowAppService : IApplicationService
{
    Task<ListResultDto<WorkflowProviderDescriptorDto>> GetProvidersAsync();
    Task<ListResultDto<WorkflowStorageProviderInfoDto>> GetStorageProvidersAsync();
}
