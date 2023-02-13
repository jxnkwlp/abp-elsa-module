using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Passingwind.WorkflowApp.Account;
using Passingwind.WorkflowApp.Permissions;
using Passingwind.WorkflowApp.Settings;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Passingwind.WorkflowApp.SettingManagement;

[Authorize(Policy = OAuth2SettingsPermissions.OAuth2)]
public class OAuth2SettingsAppService : WorkflowAppAppService, IOAuth2SettingsAppService
{
    private readonly ISettingManager _settingManager;
    private readonly ISettingProvider _settingProvider;
    private readonly ExternalLoginProviderManager _externalLoginProviderManager;

    public OAuth2SettingsAppService(ISettingManager settingManager, ISettingProvider settingProvider, ExternalLoginProviderManager externalLoginProviderManager)
    {
        _settingManager = settingManager;
        _settingProvider = settingProvider;
        _externalLoginProviderManager = externalLoginProviderManager;
    }

    public async Task<OAuth2SettingsDto> GetAsync()
    {
        OAuth2SettingsDto result = new OAuth2SettingsDto()
        {
            Enabled = await _settingProvider.GetAsync<bool>(OAuth2Setting.Enabled),
            DisplayName = await _settingProvider.GetOrNullAsync(OAuth2Setting.DisplayName),
            Authority = await _settingProvider.GetOrNullAsync(OAuth2Setting.Authority),
            MetadataAddress = await _settingProvider.GetOrNullAsync(OAuth2Setting.MetadataAddress),
            ClientId = await _settingProvider.GetOrNullAsync(OAuth2Setting.ClientId),
            ClientSecret = await _settingProvider.GetOrNullAsync(OAuth2Setting.ClientSecret),
            Scope = await _settingProvider.GetOrNullAsync(OAuth2Setting.Scope),
        };

        return result;
    }

    public async Task UpdateAsync(OAuth2SettingUpdateDto input)
    {
        await _settingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, OAuth2Setting.Enabled, input.Enabled.ToString());
        await _settingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, OAuth2Setting.DisplayName, input.DisplayName);
        await _settingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, OAuth2Setting.Authority, input.Authority);
        await _settingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, OAuth2Setting.MetadataAddress, input.MetadataAddress);
        await _settingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, OAuth2Setting.ClientId, input.ClientId);
        await _settingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, OAuth2Setting.ClientSecret, input.ClientSecret);
        await _settingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, OAuth2Setting.Scope, input.Scope);

        Logger.LogInformation("Updated OAuth2 configuration.");

        if (input.Enabled)
        {
            string[] scopes = input.Scope?.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
            await _externalLoginProviderManager.RegisterOpenIdConnectAsync(OpenIdConnectDefaults.AuthenticationScheme, input.DisplayName, options =>
            {
                options.Authority = input.Authority;
                options.MetadataAddress = input.MetadataAddress;
                options.ClientId = input.ClientId;
                options.ClientSecret = input.ClientSecret;

                foreach (string item in scopes)
                {
                    if (!options.Scope.Contains(item))
                        options.Scope.Add(item);
                }
            });
        }
        else
        {
            await _externalLoginProviderManager.UnRegisterOpenIdConnectAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
