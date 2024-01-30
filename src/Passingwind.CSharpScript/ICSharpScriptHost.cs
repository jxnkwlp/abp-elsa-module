using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;

namespace Passingwind.CSharpScriptEngine;

public interface ICSharpScriptHost
{
    Task<EmitResult?> CompileAsync(CSharpScriptCompileContext context, CancellationToken cancellationToken = default);

    Task<object?> RunAsync(CSharpScriptContext context, Action<ScriptOptions>? scriptOptions = null, CancellationToken cancellationToken = default);
}
