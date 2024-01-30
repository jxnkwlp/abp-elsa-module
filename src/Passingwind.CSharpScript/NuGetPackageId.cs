namespace Passingwind.CSharpScriptEngine;
public struct NuGetPackageId
{
    public string Id;
    public string Version;

    public NuGetPackageId(string id, string version)
    {
        Id = id;
        Version = version;
    }
}
