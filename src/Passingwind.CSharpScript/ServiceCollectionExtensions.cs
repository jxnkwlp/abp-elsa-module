using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Passingwind.CSharpScriptEngine.References;

namespace Passingwind.CSharpScriptEngine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCSharpScriptEngine(this IServiceCollection services)
    {
        services.AddOptions<CSharpScriptEngineOptions>();

        services.TryAddSingleton<ICSharpScriptHost, CSharpScriptHost>();

        services.TryAddSingleton<IScriptDirectiveReferenceResolverFactory, DefaultScriptDirectiveReferenceResolverFactory>();
        services.TryAddSingleton<IScriptDirectiveReferenceResolver<NuGetDirectiveReference>, NuGetReferenceResolver>();

        services.TryAddSingleton<IScriptReferenceResolver, DefaultScriptReferenceResolver>();

        services.TryAddSingleton<ICSharpScriptWorkspace, CSharpScriptWorkspace>();

        services.TryAddSingleton<NuGetLogger>();
        services.TryAddSingleton<INuGetPackageService, NuGetPackageService>();

        return services;
    }
}
