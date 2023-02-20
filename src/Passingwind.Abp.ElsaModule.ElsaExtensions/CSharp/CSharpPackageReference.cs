using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Passingwind.Abp.ElsaModule.CSharp;

public abstract class CSharpPackageReference
{
    public abstract Task<ImmutableArray<MetadataReference>> GetReferencesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
