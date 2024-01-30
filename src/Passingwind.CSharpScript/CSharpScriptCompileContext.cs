using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptCompileContext : CSharpScriptContext
{
    public CSharpScriptCompileContext(string sourceText, CSharpScriptEvaluationGlobal? evaluationGlobal = null, List<Assembly>? assemblies = null, List<string>? imports = null) : base(sourceText, evaluationGlobal, assemblies, imports)
    {
    }
}
