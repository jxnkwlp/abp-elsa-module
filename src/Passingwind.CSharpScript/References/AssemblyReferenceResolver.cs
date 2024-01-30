using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Passingwind.CSharpScriptEngine.References;

public class AssemblyReferenceResolver : IScriptDirectiveReferenceResolver<AssemblyReference>
{
    public Task<ImmutableArray<MetadataReference>> GetReferencesAsync(AssemblyReference reference, CancellationToken cancellationToken = default)
    {
        if (reference is null)
        {
            throw new ArgumentNullException(nameof(reference));
        }

        var assembly = Assembly.Load(reference.Name);

        var metadata = MetadataReference.CreateFromFile(assembly.Location);

        return Task.FromResult(ImmutableArray.Create<MetadataReference>(metadata));
    }
}
