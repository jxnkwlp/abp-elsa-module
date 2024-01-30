using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Passingwind.CSharpScriptEngine.Roslyn;

public interface IRoslynHost
{
    DocumentId CreateOrUpdateDocument(string projectName, string name, string text, bool isScript = false);
    Document GetDocument(string projectName, string name);
    Document GetDocument(string projectName, DocumentId documentId);
    void DeleteDocument(string projectName, string name);

    RoslynProject GetOrCreateProject(string projectName, IEnumerable<Assembly>? additionalAssemblies = null, IEnumerable<string>? additionalImports = null);
    RoslynProject GetRoslynProject(string projectName);
    void DeleteProject(string projectName);

    Task AnalysisProjectAsync(string projectName, CancellationToken cancellationToken = default);

    Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string projectName, CancellationToken cancellationToken = default);
    Task<ImmutableArray<Diagnostic>> GetDocumentDiagnosticsAsync(string projectName, string name, CancellationToken cancellationToken = default);

    Task<CSharpCompilation> GetCompilationAsync(string projectName, CancellationToken cancellationToken = default);

    void Dispose();

    TService GetService<TService>();
}
