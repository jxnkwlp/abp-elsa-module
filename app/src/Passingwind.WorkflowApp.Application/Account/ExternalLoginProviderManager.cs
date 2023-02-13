using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Passingwind.WorkflowApp.Settings;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

namespace Passingwind.WorkflowApp.Account;

public class ExternalLoginProviderManager : IExternalLoginProviderManager
{
    private readonly IAbpLazyServiceProvider _abpLazyServiceProvider;
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
    private readonly ISettingProvider _settingProvider;
    private readonly OpenIdConnectPostConfigureOptions _openIdConnectPostConfigureOptions;

    public ExternalLoginProviderManager(IAbpLazyServiceProvider abpLazyServiceProvider, IAuthenticationSchemeProvider authenticationSchemeProvider, ISettingProvider settingProvider, OpenIdConnectPostConfigureOptions openIdConnectPostConfigureOptions)
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
        _authenticationSchemeProvider = authenticationSchemeProvider;
        _settingProvider = settingProvider;
        _openIdConnectPostConfigureOptions = openIdConnectPostConfigureOptions;
    }

    public async Task RegisterAllAsync()
    {
        bool enabled = await _settingProvider.GetAsync<bool>(OAuth2Setting.Enabled);

        if (enabled)
        {
            string displayName = await _settingProvider.GetOrNullAsync(OAuth2Setting.DisplayName);
            string authority = await _settingProvider.GetOrNullAsync(OAuth2Setting.Authority);
            string metadataAddress = await _settingProvider.GetOrNullAsync(OAuth2Setting.MetadataAddress);
            string clientId = await _settingProvider.GetOrNullAsync(OAuth2Setting.ClientId);
            string clientSecret = await _settingProvider.GetOrNullAsync(OAuth2Setting.ClientSecret);
            string scope = await _settingProvider.GetOrNullAsync(OAuth2Setting.Scope);

            await RegisterOpenIdConnectAsync(OpenIdConnectDefaults.AuthenticationScheme, displayName, options =>
            {
                options.Authority = authority;
                options.MetadataAddress = metadataAddress;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });
        }
    }

    public Task RegisterAsync<TOptions, THandler>(string provider, string diskplayName, TOptions options) where TOptions : class
    {
        IOptionsMonitorCache<TOptions> optionCache = _abpLazyServiceProvider.LazyGetRequiredService<IOptionsMonitorCache<TOptions>>();

        // remove
        _authenticationSchemeProvider.RemoveScheme(provider);
        optionCache.TryRemove(provider);

        // add 
        optionCache.TryAdd(provider, options);
        AuthenticationScheme scheme = new AuthenticationScheme(provider, diskplayName, typeof(THandler));
        bool result = _authenticationSchemeProvider.TryAddScheme(scheme);

        if (!result)
        {
            throw new UserFriendlyException($"Update authentication scheme of '{provider}' failed.");
        }

        return Task.CompletedTask;
    }

    public Task UnRegisterAsync<TOptions>(string provider) where TOptions : class
    {
        IOptionsMonitorCache<TOptions> optionCache = _abpLazyServiceProvider.LazyGetRequiredService<IOptionsMonitorCache<TOptions>>();

        // remove
        _authenticationSchemeProvider.RemoveScheme(provider);
        optionCache.TryRemove(provider);

        return Task.CompletedTask;
    }

    public async Task RegisterOpenIdConnectAsync(string provider, string displayName, Action<OpenIdConnectOptions> configure)
    {
        OpenIdConnectOptions options = new OpenIdConnectOptions
        {
            RequireHttpsMetadata = false,
            GetClaimsFromUserInfoEndpoint = true,
            SaveTokens = true,
            ResponseType = OpenIdConnectResponseType.Code,
            MapInboundClaims = true
        };

        options.ClaimActions.MapAbpClaimTypes();

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        configure(options);

        _openIdConnectPostConfigureOptions.PostConfigure(provider, options);

        options.Validate();

        await RegisterAsync<OpenIdConnectOptions, OpenIdConnectHandler>(provider, displayName, options);
    }

    public async Task UnRegisterOpenIdConnectAsync(string provider)
    {
        await UnRegisterAsync<OpenIdConnectOptions>(provider);
    }
}
