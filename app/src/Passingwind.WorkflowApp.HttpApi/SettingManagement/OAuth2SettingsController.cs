using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.WorkflowApp.Controllers;
using Volo.Abp.SettingManagement;

namespace Passingwind.WorkflowApp.SettingManagement;

[Area(SettingManagementRemoteServiceConsts.ModuleName)]
[Route("api/setting-management/oauth2")]
public class OAuth2SettingsController : WorkflowAppController, IOAuth2SettingsAppService
{
    private readonly IOAuth2SettingsAppService _service;

    public OAuth2SettingsController(IOAuth2SettingsAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<OAuth2SettingsDto> GetAsync()
    {
        return _service.GetAsync();
    }

    [HttpPost]
    public Task UpdateAsync(OAuth2SettingUpdateDto input)
    {
        return _service.UpdateAsync(input);
    }
}
