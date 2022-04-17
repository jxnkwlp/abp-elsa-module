using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Designer
{
    [RemoteService]
    [Route("api/designer")]
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

        [HttpGet("script-type-definitions/{id}")]
        public Task<IRemoteStreamContent> GetScriptTypeDefinitionAsync(Guid id)
        {
            return _service.GetScriptTypeDefinitionAsync(id);
        }
    }
}
