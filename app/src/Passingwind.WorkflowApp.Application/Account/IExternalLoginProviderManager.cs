using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.DependencyInjection;

namespace Passingwind.WorkflowApp.Account;

public interface IExternalLoginProviderManager : ITransientDependency
{
    Task RegisterAllAsync();

    Task RegisterAsync<TOptions, THandler>(string provider, string diskplayName, TOptions options) where TOptions : class;
    Task UnRegisterAsync<TOptions>(string provider) where TOptions : class;

    Task RegisterOpenIdConnectAsync(string provider, string displayName, Action<OpenIdConnectOptions> configure);
    Task UnRegisterOpenIdConnectAsync(string provider);
}