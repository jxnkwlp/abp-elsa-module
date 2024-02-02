using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Passingwind.CSharpScriptEngine;

public interface ICSharpScriptWorkspace
{
    void Dispose();

    ICSharpScriptProject GetOrCreateProject(string projectId);

    Task<CSharpCompilation> CreateCompilationAsync(ICSharpScriptProject scriptProject, CancellationToken cancellationToken = default);

    Task<EmitResult> CompileAsync(ICSharpScriptProject scriptProject, CancellationToken cancellationToken = default);
}
