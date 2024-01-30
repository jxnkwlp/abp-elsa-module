using System;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.CSharpScriptEngine.References;

namespace Passingwind.CSharpScriptEngine;

public class DefaultScriptDirectiveReferenceResolverFactory : IScriptDirectiveReferenceResolverFactory
{
    private readonly IServiceProvider _services;

    public DefaultScriptDirectiveReferenceResolverFactory(IServiceProvider services)
    {
        _services = services;
    }

    public IScriptDirectiveReferenceResolver<TDirective> CreateResolver<TDirective>(TDirective directive) where TDirective : ScriptDirectiveReference
    {
        return _services.GetRequiredService<IScriptDirectiveReferenceResolver<TDirective>>();
    }
}
