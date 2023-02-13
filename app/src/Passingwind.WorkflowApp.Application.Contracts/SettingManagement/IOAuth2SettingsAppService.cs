using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Passingwind.WorkflowApp.SettingManagement;

public interface IOAuth2SettingsAppService : IApplicationService
{
    Task<OAuth2SettingsDto> GetAsync();
    Task UpdateAsync(OAuth2SettingUpdateDto input);
}
