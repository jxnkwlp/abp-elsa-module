using Microsoft.CodeAnalysis.Scripting;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptContextState
{
    public CSharpScriptContext Context { get; }

    public CSharpScriptContextState(CSharpScriptContext context)
    {
        Context = context;
    }

    public Script Script { get; set; }
}
