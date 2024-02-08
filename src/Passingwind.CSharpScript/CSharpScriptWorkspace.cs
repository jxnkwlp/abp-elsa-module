using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptWorkspace : ICSharpScriptWorkspace, IDisposable
{
    #region static

    public static ImmutableArray<string> PreprocessorSymbols => ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

    public static ImmutableArray<Assembly> DefaultCompositionAssemblies =>
        ImmutableArray.Create(
            typeof(WorkspacesResources).Assembly,
            typeof(CSharpWorkspaceResources).Assembly,
            typeof(FeaturesResources).Assembly,
            typeof(CSharpFeaturesResources).Assembly,
            typeof(CSharpScriptWorkspace).Assembly);

    //public static ImmutableArray<Type> DefaultCompositionTypes =>
    //    DefaultCompositionAssemblies.SelectMany(t => t.GetExportedTypes())
    //    // .Concat(GetDiagnosticCompositionTypes())
    //    .ToImmutableArray();

    //public static IEnumerable<Type> GetDiagnosticCompositionTypes() => typeof(AdhocWorkspace).Assembly.GetExportedTypes().Where(x => x.Namespace.StartsWith("Microsoft.CodeAnalysis.Diagnostics") || x.Namespace.StartsWith("Microsoft.CodeAnalysis.CodeFixes")).ToList();

    public static ParseOptions DefaultParseOptions => CSharpScriptHost.DefaultParseOptions;

    public static ImmutableArray<string> DefaultSuppressedDiagnostics => ImmutableArray.Create("CS1701", "CS1702", "CS1705", "CS7011", "CS8097");

    public static ImmutableArray<string> DefaultImports => CSharpScriptHost.DefaultImports;

    public static ImmutableArray<MetadataReference> DefaultReferences => CSharpScriptHost.DefaultRuntimeMetadataReference.Concat(CSharpScriptHost.StandardPortableReference).ToImmutableArray();

    #endregion static

    private readonly CompositionHost _compositionHost;
    private readonly HostServices _hostServices;
    private readonly AdhocWorkspace _workspace;
    private readonly Dictionary<string, ReportDiagnostic> _specificDiagnosticOptions = new Dictionary<string, ReportDiagnostic>();
    private static readonly ConcurrentDictionary<string, ICSharpScriptProject> Projects = new ConcurrentDictionary<string, ICSharpScriptProject>();

    private readonly IScriptReferenceResolver _scriptReferenceResolver;

    public CSharpScriptWorkspace(IScriptReferenceResolver scriptReferenceResolver)
    {
        _scriptReferenceResolver = scriptReferenceResolver;

        // nullable diagnostic options should be set to errors
        for (var i = 8600; i <= 8655; i++)
        {
            _specificDiagnosticOptions.Add($"CS{i}", ReportDiagnostic.Error);
        }

        _compositionHost = new ContainerConfiguration()
                .WithAssemblies(DefaultCompositionAssemblies)
                .CreateContainer();

        // MefHostServices.DefaultAssemblies

        _hostServices = MefHostServices.Create(_compositionHost);
        _workspace = new AdhocWorkspace(_hostServices);
    }

    public TService GetService<TService>()
    {
        return _compositionHost.GetExport<TService>();
    }

    public ICSharpScriptProject GetOrCreateProject(string projectId)
    {
        return Projects.GetOrAdd(projectId, (_) =>
        {
            var compilationOptions = CreateDefaultCompilationOptions();
            var csProject = CreateProject(_workspace, _, compilationOptions);
            return new CSharpScriptProject(_workspace, csProject.Id, csProject.Name);
        });
    }

    protected Project CreateProject(AdhocWorkspace workspace, string projectName, CSharpCompilationOptions compilationOptions)
    {
        var references = DefaultReferences.ToList();

        var analyzerReferences = GetSolutionAnalyzerReferences();

        var projectId = ProjectId.CreateNewId();

        var root = GetProjectFolder(projectName);
        var outputFilePath = Path.Combine(root, "bin", projectName + ".dll");

        var projectInfo = ProjectInfo.Create(
            id: projectId,
            version: VersionStamp.Create(),
            name: projectName,
            assemblyName: projectName,
            language: LanguageNames.CSharp,
            filePath: root,
            outputFilePath: outputFilePath,
            compilationOptions: compilationOptions,
            parseOptions: DefaultParseOptions,
            metadataReferences: references,
            analyzerReferences: analyzerReferences
        );

        var project = workspace.AddProject(projectInfo);

        // auto add global using 
        workspace.AddDocument(projectId, "GlobalUsings", SourceText.From(CSharpScriptWorkspace.GenerateGlobalUsings(compilationOptions.Usings.ToArray())));

        workspace.TryApplyChanges(workspace.CurrentSolution);

        return project;
    }

    protected virtual IEnumerable<AnalyzerReference> GetSolutionAnalyzerReferences()
    {
        var loader = GetService<IAnalyzerAssemblyLoader>();
        yield return new AnalyzerFileReference(typeof(Compilation).Assembly.Location, loader);
        yield return new AnalyzerFileReference(typeof(CSharpResources).Assembly.Location, loader);
        yield return new AnalyzerFileReference(typeof(FeaturesResources).Assembly.Location, loader);
        yield return new AnalyzerFileReference(typeof(CSharpFeaturesResources).Assembly.Location, loader);
    }

    private static string GetProjectFolder(string project)
    {
        var root = Path.Combine(Path.GetTempPath(), "workflow", "roslyns", project);
        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }

        return root;
    }

    public void Dispose()
    {
        Projects.Clear();
        _workspace.ClearSolution();
        _workspace.Dispose();
        _compositionHost.Dispose();
    }

    protected static string GenerateGlobalUsings(params string[] imports)
    {
        return string.Join("\r\n", imports.Select(x => $"global using {x};"));
    }

    protected CSharpCompilationOptions CreateDefaultCompilationOptions(IEnumerable<string>? additionalImports = null)
    {
        var imports = DefaultImports.Concat(additionalImports ?? Array.Empty<string>());

        return new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            reportSuppressedDiagnostics: false,
            scriptClassName: "Program",
            usings: imports,
            checkOverflow: true,
            allowUnsafe: false,
            // generalDiagnosticOption: ReportDiagnostic.Default,
            specificDiagnosticOptions: _specificDiagnosticOptions,
            concurrentBuild: false,
            metadataReferenceResolver: NuGetMetadataReferenceResolver.Instance,
            nullableContextOptions: NullableContextOptions.Enable);
    }

    public async Task<CSharpCompilation> CreateCompilationAsync(ICSharpScriptProject scriptProject, bool removeReferenceDirective = false, CancellationToken cancellationToken = default)
    {
        var scriptOptions = CSharpScriptHost.CreateScriptOptions(scriptProject.GetImports(), scriptProject.GetReferences());
        var parseOptions = CSharpScriptHost.CreateParseOptions();

        scriptOptions = scriptOptions.AddReferences(CSharpScriptHost.StandardPortableReference);

        var project = _workspace.CurrentSolution.GetProject(scriptProject.ProjectId) ?? throw new InvalidOperationException("Project not found.");

        var syntaxTrees = new List<SyntaxTree>();

        foreach (var item in project.Documents)
        {
            var sourceText = (await item.GetTextAsync(cancellationToken)).ToString();

            // resolve references
            scriptOptions = scriptOptions.AddReferences(await _scriptReferenceResolver.ResolveReferencesAsync(sourceText, cancellationToken));

            var syntaxTree = await item.GetSyntaxTreeAsync(cancellationToken);

            if (removeReferenceDirective)
            {
                syntaxTree = syntaxTree!.RemoveReferenceDirectives(parseOptions, cancellationToken: cancellationToken);
            }

            syntaxTrees.Add(syntaxTree!);
        }

        var name = scriptProject.Name;

        var compilationOptions = CreateDefaultCompilationOptions(scriptOptions.Imports);

        return CSharpCompilation.Create(name, syntaxTrees: syntaxTrees, references: scriptOptions.MetadataReferences, options: compilationOptions);
    }

    public async Task<EmitResult> CompileAsync(ICSharpScriptProject scriptProject, CancellationToken cancellationToken = default)
    {
        var name = scriptProject.Name;

        var project = _workspace.CurrentSolution.GetProject(scriptProject.ProjectId) ?? throw new InvalidOperationException("Project not found.");

        var compilation = await CreateCompilationAsync(scriptProject, true, cancellationToken);

        var rootFolder = GetProjectFolder(name);

        string outputFilePath = project.OutputFilePath ?? Path.Combine(rootFolder, "bin", $"{name}.dll");

        if (!Directory.Exists(Path.GetDirectoryName(outputFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)!);
        }

        await using var peStream = File.OpenWrite(outputFilePath);
        await using var pdbStream = File.OpenWrite(Path.ChangeExtension(outputFilePath, ".pdb"));

        return compilation.Emit(peStream, pdbStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb), cancellationToken: cancellationToken);
    }

    public async Task RestoreReferenceAsync(ICSharpScriptProject scriptProject, CancellationToken cancellationToken = default)
    {
        var projectId = scriptProject.ProjectId;
        var project = _workspace.CurrentSolution.GetProject(scriptProject.ProjectId) ?? throw new InvalidOperationException("Project not found.");

        foreach (var document in project.Documents)
        {
            var sourceText = (await document.GetTextAsync(cancellationToken)).ToString();

            var references = await _scriptReferenceResolver.ResolveReferencesAsync(sourceText, cancellationToken);

            foreach (var item in references)
            {
                if (!project.MetadataReferences.Any(x => IsSameMetadataReference(x, item)))
                {
                    var solution = _workspace.CurrentSolution.AddMetadataReferences(projectId, references);

                    _workspace.TryApplyChanges(solution);
                }
            }
        }

        scriptProject.ReloadProject();

        bool IsSameMetadataReference(MetadataReference a, MetadataReference b)
        {
            if (a.GetType() != b.GetType())
            {
                return false;
            }

            return a is PortableExecutableReference executableReferenceA && b is PortableExecutableReference executableReferenceB && executableReferenceA.FilePath == executableReferenceB.FilePath;
        }
    }

    public async Task<IReadOnlyList<CompletionItem>> GetCompletionsAsync(ICSharpScriptProject scriptProject, DocumentId documentId, int position, bool filter = true, CancellationToken cancellationToken = default)
    {
        var projectId = scriptProject.ProjectId;
        var project = _workspace.CurrentSolution.GetProject(scriptProject.ProjectId) ?? throw new InvalidOperationException("Project not found.");

        var document = await project.GetDocumentAsync(documentId, cancellationToken: cancellationToken) ?? throw new InvalidOperationException("The document not found.");
        var completionService = CompletionService.GetService(document);

        if (completionService == null)
        {
            return new List<CompletionItem>();
        }

        var completionResult = await completionService.GetCompletionsAsync(document, position, cancellationToken: cancellationToken);

        var text = await document.GetTextAsync(cancellationToken);

        var textSpanToText = new Dictionary<TextSpan, string>();

        if (!filter)
        {
            return completionResult.ItemsList;
        }

        return completionResult.ItemsList.Where(item => MatchesFilterText(completionService, document, item, text, textSpanToText)).ToList();
    }

    private static bool MatchesFilterText(CompletionService completionService, Document document, CompletionItem item, SourceText text, Dictionary<TextSpan, string> textSpanToText)
    {
        var filterText = GetFilterText(item, text, textSpanToText);
        if (string.IsNullOrEmpty(filterText))
        {
            return true;
        }

        return completionService.FilterItems(document, ImmutableArray.Create(item), filterText).Length > 0;
    }

    private static string GetFilterText(CompletionItem item, SourceText text, Dictionary<TextSpan, string> textSpanToText)
    {
        var textSpan = item.Span;
        if (!textSpanToText.TryGetValue(textSpan, out var filterText))
        {
            filterText = text.GetSubText(textSpan).ToString();
            textSpanToText[textSpan] = filterText;
        }
        return filterText;
    }

    public async Task<Document?> GetDocumentAsync(DocumentId documentId)
    {
        return await _workspace.CurrentSolution.GetDocumentAsync(documentId);
    }
}
