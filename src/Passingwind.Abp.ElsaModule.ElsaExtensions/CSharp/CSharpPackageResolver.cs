using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class CSharpPackageResolver : ICSharpPackageResolver
{
    private readonly ILogger<CSharpPackageResolver> _logger;
    private readonly INuGetPackageResolver _nuGetPackageResolver;

    public CSharpPackageResolver(ILogger<CSharpPackageResolver> logger, INuGetPackageResolver nuGetPackageResolver)
    {
        _logger = logger;
        _nuGetPackageResolver = nuGetPackageResolver;
    }

    public async ValueTask<ImmutableArray<CSharpPackageReference>> ResolveAsync(string text, CancellationToken cancellationToken = default)
    {
        var packages = new List<CSharpPackageReference>();

        using StringReader sr = new(text?.Trim());
        string line = string.Empty;
        while (!string.IsNullOrWhiteSpace(line = sr.ReadLine()))
        {
            var tmp = line.Trim();
            if (tmp.StartsWith("#r"))
            {
                var directive = tmp[2..].Trim().Trim('"');

                if (directive.StartsWith("nuget:"))
                {
                    var nugetPackage = await _nuGetPackageResolver.ResolveAsync(tmp[2..].Trim().Trim('"'), cancellationToken);
                    packages.Add(nugetPackage);

                    _logger.LogInformation("Resolved nuget package directive '{nugetPackage}' ", nugetPackage);
                }
                // TODO other directives
                // 
            }
            else
            {
                break;
            }
        }

        return packages.ToImmutableArray();
    }
}
