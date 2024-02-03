using Microsoft.CodeAnalysis.Scripting;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptContextState
{
    public CSharpScriptContext Context { get; } = null!;
    public Script Script { get; set; } = null!;

    public CSharpScriptContextState(CSharpScriptContext context)
    {
        Context = context;
    }
}
