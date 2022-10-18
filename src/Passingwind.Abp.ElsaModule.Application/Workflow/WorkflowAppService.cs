using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Providers.Workflows;
using Elsa.Services.WorkflowStorage;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    [Authorize]
    public class WorkflowAppService : ElsaModuleAppService, IWorkflowAppService
    {
        private readonly IWorkflowStorageService _workflowStorageService;
        private readonly IEnumerable<IWorkflowProvider> _workflowProviders;

        public WorkflowAppService(IWorkflowStorageService workflowStorageService, IEnumerable<IWorkflowProvider> workflowProviders)
        {
            _workflowStorageService = workflowStorageService;
            _workflowProviders = workflowProviders;
        }

        public Task<ListResultDto<WorkflowStorageProviderInfoDto>> GetStorageProvidersAsync()
        {
            var all = _workflowStorageService.ListProviders();

            var list = all.Select(x => new WorkflowStorageProviderInfoDto { Name = x.Name, DisplayName = x.DisplayName }).ToList();

            return Task.FromResult(new ListResultDto<WorkflowStorageProviderInfoDto>(list));
        }

        public Task<ListResultDto<WorkflowProviderDescriptorDto>> GetProvidersAsync()
        {
            var items = _workflowProviders.Select(x => new WorkflowProviderDescriptorDto
            {
                Name = x.GetType().Name,
                Type = x.GetType().Name.Humanize(),
            }).ToArray();

            return Task.FromResult(new ListResultDto<WorkflowProviderDescriptorDto>(items));
        }
    }
}
