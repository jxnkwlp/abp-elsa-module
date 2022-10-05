using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public interface IDesignerAppService : IApplicationService
    {
        Task<ActivityTypeDescriptorListResultDto> GetActivityTypesAsync();

        Task<RuntimeSelectListResultDto> GetRuntimeSelectListItems(RuntimeSelectListContextDto input);

        Task<IRemoteStreamContent> GetScriptTypeDefinitionAsync(Guid id);
    }
}
