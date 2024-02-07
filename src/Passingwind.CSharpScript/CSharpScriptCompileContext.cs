using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptCompileContext : CSharpScriptContext
{
    public CSharpScriptCompileContext(ILogger logger, string sourceText, CSharpScriptEvaluationGlobal? evaluationGlobal = null, List<Assembly>? assemblies = null, List<string>? imports = null) : base(logger, sourceText, evaluationGlobal, assemblies, imports)
    {
    }
}
