namespace Passingwind.CSharpScriptEngine.References;

public class AssemblyReference : ScriptDirectiveReference
{
    public string Name { get; set; }

    public AssemblyReference(string name)
    {
        Name = name;
    }
}
