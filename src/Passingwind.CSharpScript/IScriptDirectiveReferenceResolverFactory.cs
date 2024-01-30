using Passingwind.CSharpScriptEngine.References;

namespace Passingwind.CSharpScriptEngine;

public interface IScriptDirectiveReferenceResolverFactory
{
    IScriptDirectiveReferenceResolver<TDirective> CreateResolver<TDirective>(TDirective directive) where TDirective : ScriptDirectiveReference;
}
