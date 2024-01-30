using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NuGet.Packaging.Core;

namespace Passingwind.CSharpScriptEngine;

public class NuGetReference : IEquatable<NuGetReference>
{
    public NuGetReference(PackageIdentity identity, ImmutableArray<string> references)
    {
        Identity = identity;
        References = references;
    }

    public PackageIdentity Identity { get; }

    public ImmutableArray<string> References { get; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as NuGetReference);
    }

    public bool Equals(NuGetReference? other)
    {
        return other is not null &&
               EqualityComparer<PackageIdentity>.Default.Equals(Identity, other.Identity);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Identity);
    }

    public static bool operator ==(NuGetReference left, NuGetReference right)
    {
        return EqualityComparer<NuGetReference>.Default.Equals(left, right);
    }

    public static bool operator !=(NuGetReference left, NuGetReference right)
    {
        return !(left == right);
    }
}
