using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Passingwind.CSharpScriptEngine.References;

public class LocalFileReferenceResolver : IScriptDirectiveReferenceResolver<LocalFileDirectiveReference>
{
    public Task<ImmutableArray<MetadataReference>> GetReferencesAsync(LocalFileDirectiveReference reference, CancellationToken cancellationToken = default)
    {
        if (reference is null)
        {
            throw new ArgumentNullException(nameof(reference));
        }

        if (!File.Exists(reference.Path))
        {
            throw new FileNotFoundException(reference.Path, "The reference file not found.");
        }

        var metadata = MetadataReference.CreateFromFile(reference.Path);

        return Task.FromResult(ImmutableArray.Create<MetadataReference>(metadata));
    }
}
