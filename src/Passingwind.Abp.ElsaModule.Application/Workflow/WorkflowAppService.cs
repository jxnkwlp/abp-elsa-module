using System.Linq;
using System.Threading.Tasks;
using Elsa.Services.WorkflowStorage;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public class WorkflowAppService : ElsaModuleAppService, IWorkflowAppService
    {
        private readonly IWorkflowStorageService _workflowStorageService;

        public WorkflowAppService(IWorkflowStorageService workflowStorageService)
        {
            _workflowStorageService = workflowStorageService;
        }

        public Task<ListResultDto<WorkflowStorageProviderInfoDto>> GetStorageProvidersAsync()
        {
            var all = _workflowStorageService.ListProviders();

            var list = all.Select(x => new WorkflowStorageProviderInfoDto { Name = x.Name, DisplayName = x.DisplayName }).ToList();

            return Task.FromResult(new ListResultDto<WorkflowStorageProviderInfoDto>(list));
        }
    }
}
