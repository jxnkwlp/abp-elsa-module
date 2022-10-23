using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    [RemoteService]
    [Route("api/elsa/designer")]
    public class DesignerController : ElsaModuleController, IDesignerAppService
    {
        private readonly IDesignerAppService _service;

        public DesignerController(IDesignerAppService service)
        {
            _service = service;
        }

        [HttpGet("activity-types")]
        public Task<ActivityTypeDescriptorListResultDto> GetActivityTypesAsync()
        {
            return _service.GetActivityTypesAsync();
        }

        [HttpPost("runtime-select-list")]
        public Task<RuntimeSelectListResultDto> GetRuntimeSelectListItems(RuntimeSelectListContextDto input)
        {
            return _service.GetRuntimeSelectListItems(input);
        }

        [HttpGet("scripting/javascript/type-definitions/{id}")]
        public Task<IRemoteStreamContent> GetScriptTypeDefinitionAsync(Guid id)
        {
            return _service.GetScriptTypeDefinitionAsync(id);
        }
    }
}
