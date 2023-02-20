using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public interface ICSharpPackageResolver : ITransientDependency
{
    /// <summary>
    ///  resolve references from text
    /// </summary> 
    ValueTask<ImmutableArray<CSharpPackageReference>> ResolveAsync(string text, CancellationToken cancellationToken = default);
}
