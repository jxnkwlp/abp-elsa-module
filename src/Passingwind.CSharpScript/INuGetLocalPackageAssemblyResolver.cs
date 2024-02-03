using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;

namespace Passingwind.CSharpScriptEngine;

public interface INuGetLocalPackageAssemblyResolver
{
    Task<IEnumerable<string>> GetReferencesAsync(PackageIdentity package, NuGetFramework framework, CancellationToken cancellationToken = default);
}
