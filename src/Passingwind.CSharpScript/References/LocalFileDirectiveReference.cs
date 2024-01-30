using Passingwind.CSharpScriptEngine.References;

namespace Passingwind.CSharpScriptEngine;

public class LocalFileDirectiveReference : ScriptDirectiveReference
{
    public string Path { get; }

    public LocalFileDirectiveReference(string path)
    {
        Path = path;
    }
}
