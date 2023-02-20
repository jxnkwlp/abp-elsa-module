using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public interface INuGetPackageResolver : ISingletonDependency
{
    Task<NuGetPackageReference> ResolveAsync(string text, CancellationToken cancellationToken = default);
}
