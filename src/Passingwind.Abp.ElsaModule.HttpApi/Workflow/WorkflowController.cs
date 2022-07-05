using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    [RemoteService]
    [Route("api/workflows")]
    public class WorkflowController : ElsaModuleController, IWorkflowAppService
    {
        private readonly IWorkflowAppService _service;

        public WorkflowController(IWorkflowAppService service)
        {
            _service = service;
        }

        [HttpGet("storage-providers")]
        public virtual Task<ListResultDto<WorkflowStorageProviderInfoDto>> GetStorageProvidersAsync()
        {
            return _service.GetStorageProvidersAsync();
        }

    }
}
