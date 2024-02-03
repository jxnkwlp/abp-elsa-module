using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Passingwind.CSharpScriptEngine;

public interface ICSharpScriptWorkspace
{
    void Dispose();

    ICSharpScriptProject GetOrCreateProject(string projectId);

    Task<CSharpCompilation> CreateCompilationAsync(ICSharpScriptProject scriptProject, bool removeReferenceDirective = false, CancellationToken cancellationToken = default);

    Task<EmitResult> CompileAsync(ICSharpScriptProject scriptProject, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompletionItem>> GetCompletionsAsync(ICSharpScriptProject scriptProject, DocumentId documentId, int position, bool filter = true, CancellationToken cancellationToken = default);

    Task RestoreReferenceAsync(ICSharpScriptProject scriptProject, CancellationToken cancellationToken = default);

    Task<Document?> GetDocumentAsync(DocumentId documentId);
}
