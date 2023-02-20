using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class NuGetPackageReference : CSharpPackageReference
{
    public string PackageId { get; }
    public string PackageVersion { get; }

    public NuGetPackageReference(string packageId, string packageVersion)
    {
        if (packageId == null)
            throw new ArgumentNullException(nameof(packageId));

        if (packageVersion == null)
            throw new ArgumentNullException(nameof(packageVersion));

        PackageId = packageId;
        PackageVersion = packageVersion;
    }

    public override async Task<ImmutableArray<MetadataReference>> GetReferencesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var nugetService = serviceProvider.GetRequiredService<INuGetPackageService>();

        // TODO
        var files = await nugetService.GetReferencesAsync(PackageId, PackageVersion, CSharpEnvironment.TargetFramework, true, false, cancellationToken);

        if (files?.Any() == true)
        {
            return files.Select(x => MetadataReference.CreateFromFile(x)).ToImmutableArray<MetadataReference>();
        }

        return ImmutableArray<MetadataReference>.Empty;
    }

    public static bool TryParse(string value, out NuGetPackageReference reference)
    {
        value = value.Trim();

        if (!value.StartsWith("nuget:"))
        {
            reference = null;
            return false;
        }
        var parts = value.Split(new char[] { ',' }, 2)
            .Select(v => v.Trim())
            .ToArray();

        if (parts.Length == 0)
        {
            reference = null;
            return false;
        }

        var packageName = parts[0].Substring(6).Trim();

        if (string.IsNullOrWhiteSpace(packageName))
        {
            reference = null;
            return false;
        }

        var packageVersion = parts.Length > 1
            ? parts[1]
            : null;

        reference = new NuGetPackageReference(packageName, packageVersion);

        return true;
    }

    public override string ToString()
    {
        return $"nuget:{PackageId}, {PackageVersion}";
    }
}