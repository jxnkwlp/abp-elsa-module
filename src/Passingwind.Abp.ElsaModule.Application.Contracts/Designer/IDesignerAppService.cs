using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Designer
{
    public interface IDesignerAppService : IApplicationService
    {
        Task<ActivityTypeDescriptorListResultDto> GetActivityTypesAsync();

        Task<IRemoteStreamContent> GetScriptTypeDefinitionAsync(Guid id);
    }
}
