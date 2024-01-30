namespace Passingwind.CSharpScriptEngine.References;

public class NuGetDirectiveReference : ScriptDirectiveReference
{
    public NuGetDirectiveReference(string packageId, string version)
    {
        PackageId = packageId;
        Version = version;
    }

    public NuGetDirectiveReference(string package)
    {
        // TODO
        PackageId = package.Split(",")[0].Trim();
        Version = package.Split(",")[1].Trim();
    }

    public string PackageId { get; }
    public string Version { get; }
}
