using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Scripting.CSharp;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;

public class CodeAnalysisProvider : IMonacoCodeAnalysisProvider
{
    private readonly ILogger<CodeAnalysisProvider> _logger;
    private readonly ICSharpSourceGenerator _iCSharpSourceGenerator;

    public CodeAnalysisProvider(ILogger<CodeAnalysisProvider> logger, ICSharpSourceGenerator iCSharpSourceGenerator)
    {
        _logger = logger;
        _iCSharpSourceGenerator = iCSharpSourceGenerator;
    }

    public async Task<MonacoCodeAnalysisResult> HandleAsync(MonacoCodeAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.SessionId is null) throw new ArgumentNullException(nameof(request.SessionId));

        var generate = await _iCSharpSourceGenerator.GenerateAsync(request.WorkflowDefinition);

        var host = RoslynHostSessionContainer.GetOrCreate(request.SessionId ?? request.WorkflowDefinition.DefinitionId, new RoslynHostReference(generate.Assemblies, generate.Imports));
        host.AddImports(generate.Imports.ToArray());
        host.AddOrUpdateDocument("Definition", generate.Code);
        host.AddOrUpdateScriptDocument(request.Code);

        var cSharpCompilation = await host.GetCompilationAsync();

        var root = Path.Combine(Path.GetTempPath(), "elsamodule", "roslyns");
        if (!Directory.Exists(root))
            Directory.CreateDirectory(root);

        var emitResult = cSharpCompilation.Emit(Path.Combine(root, Guid.NewGuid().ToString("N")), cancellationToken: cancellationToken);

        var result = new List<MonacoCodeAnalysisItem>();
        foreach (var diagnostic in emitResult.Diagnostics)
        {
            var file = diagnostic.Location?.SourceTree?.FilePath;

            if (file == null)
                continue;

            var document = host.GetDocument(file);

            if (document == null)
                continue;

            if (!host.IsScriptDocument(document))
            {
                continue;
            }

            var severity = MapDiagnosticSeverity(diagnostic);

            var keyword = (await document.GetTextAsync(cancellationToken)).GetSubText(diagnostic.Location.SourceSpan).ToString();
            var msg = new MonacoCodeAnalysisItem()
            {
                Id = diagnostic.Id,
                Keyword = keyword,
                Message = diagnostic.GetMessage(),
                OffsetFrom = diagnostic.Location.SourceSpan.Start,
                OffsetTo = diagnostic.Location.SourceSpan.End,
                Severity = severity,
                SeverityNumeric = (int)severity,
            };
            result.Add(msg);
        }
        return new MonacoCodeAnalysisResult() { Items = result };
    }

    private static MonacoCodeAnalysisSeverity MapDiagnosticSeverity(Diagnostic diagnostic)
    {
        return diagnostic.Severity switch
        {
            DiagnosticSeverity.Error => MonacoCodeAnalysisSeverity.Error,
            DiagnosticSeverity.Warning => MonacoCodeAnalysisSeverity.Warning,
            DiagnosticSeverity.Info => MonacoCodeAnalysisSeverity.Info,
            _ => MonacoCodeAnalysisSeverity.Hint
        };
    }
}
