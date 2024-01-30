using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptContext
{
    public CSharpScriptContext(string sourceText, CSharpScriptEvaluationGlobal? evaluationGlobal = null, List<Assembly>? assemblies = null, List<string>? imports = null)
    {
        SourceText = sourceText;
        EvaluationGlobal = evaluationGlobal ?? new CSharpScriptEvaluationGlobal();
        Assemblies = assemblies ?? new List<Assembly>();
        Imports = imports ?? new List<string>();
    }

    public string? ScriptId { get; set; }
    public string SourceText { get; }
    public CSharpScriptEvaluationGlobal EvaluationGlobal { get; }
    public List<Assembly> Assemblies { get; }
    public List<string> Imports { get; }

    public bool IsChanged(CSharpScriptContext other)
    {
        return !other.SourceText.Equals(SourceText);
    }
}
