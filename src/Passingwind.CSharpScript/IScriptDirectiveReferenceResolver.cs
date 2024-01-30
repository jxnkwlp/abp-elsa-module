using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Passingwind.CSharpScriptEngine.References;

namespace Passingwind.CSharpScriptEngine;

public interface IScriptDirectiveReferenceResolver<TDirective> where TDirective : ScriptDirectiveReference
{
    Task<ImmutableArray<MetadataReference>> GetReferencesAsync(TDirective reference, CancellationToken cancellationToken = default);
}
