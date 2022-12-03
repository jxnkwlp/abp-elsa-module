using System;
using Microsoft.AspNetCore.Authentication;

namespace Demo.ApiKeys;

public static class ApiKeyExtensions
{
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyOptions> configureOptions)
    {
        return builder.AddScheme<ApiKeyOptions, ApiKeyAuthenticationHandler>(authenticationScheme, configureOptions);
    }

    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyOptions> configureOptions)
    {
        return builder.AddScheme<ApiKeyOptions, ApiKeyAuthenticationHandler>(ApiKeyDefaults.AuthenticationScheme, configureOptions);
    }

    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<ApiKeyOptions, ApiKeyAuthenticationHandler>(ApiKeyDefaults.AuthenticationScheme, (options) => { });
    }
}
