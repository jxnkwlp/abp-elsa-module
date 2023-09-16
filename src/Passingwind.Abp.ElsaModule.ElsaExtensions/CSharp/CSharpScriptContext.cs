using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class CSharpScriptContext
{
    public CSharpScriptContext(string sourceText, CSharpScriptEvaluationGlobal evaluationGlobal, List<Assembly> assemblies, List<string> imports)
    {
        SourceText = sourceText;
        EvaluationGlobal = evaluationGlobal;
        Assemblies = assemblies;
        Imports = imports;
    }

    public string SourceText { get; }
    public CSharpScriptEvaluationGlobal EvaluationGlobal { get; }
    public List<Assembly> Assemblies { get; }
    public List<string> Imports { get; }

    public bool IsChanged(CSharpScriptContext other)
    {
        return !other.SourceText.Equals(SourceText);
    }
}
