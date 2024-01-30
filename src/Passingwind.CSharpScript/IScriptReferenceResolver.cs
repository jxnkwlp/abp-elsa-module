using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Passingwind.CSharpScriptEngine.References;

namespace Passingwind.CSharpScriptEngine;

public interface IScriptReferenceResolver
{
    /// <summary>
    ///  Resolve references from scripts
    /// </summary>
    Task<ImmutableArray<MetadataReference>> ResolveReferencesAsync(string scripts, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Resolve directives from scripts
    /// </summary>
    Task<ImmutableArray<ScriptDirectiveReference>> GetDirectivesAsync(string scripts, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Resolve references from directive reference
    /// </summary>
    Task<ImmutableArray<MetadataReference>> GetReferencesAsync(ScriptDirectiveReference reference, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Resolve references from directive reference
    /// </summary>
    Task<ImmutableArray<MetadataReference>> GetReferencesAsync(string reference, CancellationToken cancellationToken = default);
}
