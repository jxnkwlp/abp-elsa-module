using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Nito.AsyncEx;
using Passingwind.Abp.ElsaModule.CSharp;

namespace Passingwind.Abp.ElsaModule.Roslyn;

public class RoslynHost : IDisposable, IRoslynHost
{
    #region static

    public static ImmutableArray<string> PreprocessorSymbols => ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

    public static ImmutableArray<Assembly> DefaultCompositionAssemblies =>
        ImmutableArray.Create(
            // Microsoft.CodeAnalysis.Workspaces
            typeof(WorkspacesResources).Assembly,
            // Microsoft.CodeAnalysis.CSharp.Workspaces
            typeof(CSharpWorkspaceResources).Assembly,
            // Microsoft.CodeAnalysis.Features
            typeof(FeaturesResources).Assembly,
            // Microsoft.CodeAnalysis.CSharp.Features
            typeof(CSharpFeaturesResources).Assembly,
            typeof(RoslynHost).Assembly);

    public static ImmutableArray<Type> DefaultCompositionTypes =>
        DefaultCompositionAssemblies.SelectMany(t => t.GetTypes())
        .ToImmutableArray();

    public static ParseOptions DefaultParseOptions => CSharpParseOptions
        .Default
        .WithPreprocessorSymbols(PreprocessorSymbols)
        .WithKind(SourceCodeKind.Script)
        .CommonWithKind(SourceCodeKind.Regular);

    public static ImmutableArray<string> DefaultSuppressedDiagnostics => ImmutableArray.Create("CS1701", "CS1702", "CS1705", "CS7011", "CS8097");

    public static ImmutableArray<string> DefaultImports => ImmutableArray.Create(new[]{
        "System",
        "System.Threading",
        "System.Threading.Tasks",
        "System.Collections",
        "System.Collections.Generic",
        "System.Text",
        "System.Text.RegularExpressions",
        "System.Linq",
        "System.IO",
        "System.Reflection",
    });

    public static ImmutableArray<MetadataReference> DefaultMetadataReferences => Basic.Reference.Assemblies.Net60.References.All.ToImmutableArray<MetadataReference>();

    #endregion static

    private ParseOptions _parseOptions;
    private CompositionHost _compositionHost;
    private HostServices _hostServices;
    private AdhocWorkspace _workspace;

    private readonly ConcurrentDictionary<string, Lazy<RoslynProject>> _projects = new ConcurrentDictionary<string, Lazy<RoslynProject>>();
    private readonly Dictionary<string, ReportDiagnostic> _specificDiagnosticOptions = new Dictionary<string, ReportDiagnostic>();

    private readonly ICSharpPackageResolver _cSharpPackageResolver;
    private readonly IServiceProvider _serviceProvider;

    public RoslynHost(ICSharpPackageResolver cSharpPackageResolver, IServiceProvider serviceProvider)
    {
        _cSharpPackageResolver = cSharpPackageResolver;
        _serviceProvider = serviceProvider;
        Initial();
    }

    public void Initial()
    {
        _parseOptions = GetParseOptions();
        var partTypes = GetCompositionTypes();

        // nullable diagnostic options should be set to errors
        for (var i = 8600; i <= 8655; i++)
        {
            _specificDiagnosticOptions.Add($"CS{i}", ReportDiagnostic.Error);
        }

        _compositionHost = new ContainerConfiguration()
                .WithParts(partTypes)
                .CreateContainer();

        _hostServices = MefHostServices.Create(_compositionHost);
        _workspace = new AdhocWorkspace(_hostServices);
    }

    public void Restart()
    {
        _workspace?.Dispose();
    }

    protected void CreateDocument(ProjectId projectId, string name, string text, bool isScript = false)
    {
        lock (projectId)
        {
            var doc = _workspace.AddDocument(DocumentInfo.Create(
                   DocumentId.CreateNewId(projectId),
                   name,
                    sourceCodeKind: isScript ? SourceCodeKind.Script : SourceCodeKind.Regular,
                   loader: TextLoader.From(TextAndVersion.Create(SourceText.From(text, Encoding.UTF8), VersionStamp.Default)),
                   filePath: $"{name}.cs"));

            _workspace.TryApplyChanges(_workspace.CurrentSolution);
        }
    }

    public Document GetDocument(string projectName, string name)
    {
        var roslynProject = GetRoslynProject(projectName);
        var project = _workspace.CurrentSolution.GetProject(roslynProject.Id);
        return project.Documents.FirstOrDefault(x => x.Name == name);
    }

    public Document GetDocument(string projectName, DocumentId documentId)
    {
        var roslynProject = GetRoslynProject(projectName);
        var project = _workspace.CurrentSolution.GetProject(roslynProject.Id);
        return project.GetDocument(documentId);
    }

    public void DeleteDocument(ProjectId projectId, string name)
    {
        lock (projectId)
        {
            var project = _workspace.CurrentSolution.GetProject(projectId);
            var document = project.Documents.FirstOrDefault(x => x.Name == name);
            if (document != null)
            {
                var solution = _workspace.CurrentSolution.RemoveDocument(document.Id);
                _workspace.TryApplyChanges(solution);
            }
        }
    }

    public void DeleteDocument(string projectName, string name)
    {
        lock (projectName)
        {
            var roslynProject = GetRoslynProject(projectName);
            var project = _workspace.CurrentSolution.GetProject(roslynProject.Id);
            var document = project.Documents.FirstOrDefault(x => x.Name == name);

            if (document != null)
            {
                var solution = _workspace.CurrentSolution.RemoveDocument(document.Id);
                _workspace.TryApplyChanges(solution);
            }
        }
    }

    public DocumentId CreateOrUpdateDocument(string projectName, string name, string text, bool isScript = false)
    {
        lock (projectName)
        {
            var roslynProject = GetRoslynProject(projectName);

            var project = _workspace.CurrentSolution.GetProject(roslynProject.Id);

            var document = project.Documents.FirstOrDefault(x => x.Name == name);

            if (document != null)
            {
                var newSource = SourceText.From(text, Encoding.UTF8);

                var solution = _workspace.CurrentSolution.WithDocumentText(document.Id, SourceText.From(text, Encoding.UTF8));

                //if (document.HasTextChanged(newDocument, true))
                //{
                //    document.DocumentState.UpdateText(SourceText.From(text, Encoding.UTF8), PreservationMode.PreserveValue);
                //}

                //project = project.RemoveDocument(document.Id); 
                //_workspace.TryApplyChanges(project.Solution);

                _workspace.TryApplyChanges(solution);
            }
            else
            {
                document = _workspace.AddDocument(DocumentInfo.Create(
                   DocumentId.CreateNewId(roslynProject.Id),
                   name,
                   sourceCodeKind: isScript ? SourceCodeKind.Script : SourceCodeKind.Regular,
                   loader: TextLoader.From(TextAndVersion.Create(SourceText.From(text, Encoding.UTF8), VersionStamp.Default)),
                   filePath: $"{name}.cs"));
            }

            _workspace.TryApplyChanges(project.Solution);

            return document.Id;
        }
    }

    public RoslynProject GetOrCreateProject(string projectName, IEnumerable<Assembly> additionalAssemblies = null, IEnumerable<string> additionalImports = null)
    {
        return _projects.GetOrAdd(projectName, (key) =>
        {
            return new Lazy<RoslynProject>(() =>
            {
                var project = CreateProject(_workspace, projectName, additionalAssemblies, additionalImports);

                return new RoslynProject(
                     key,
                     project.Id,
                     additionalAssemblies == null ? ImmutableArray<Assembly>.Empty : additionalAssemblies.ToImmutableArray(),
                     additionalImports == null ? ImmutableArray<string>.Empty : additionalImports.ToImmutableArray());
            });
        }).Value;
    }

    public RoslynProject GetRoslynProject(string projectName)
    {
        if (_projects.TryGetValue(projectName, out var project))
        {
            return project.Value;
        }
        throw new Exception($"The project '{projectName}' not found.");
    }

    public void DeleteProject(string projectName)
    {
        _projects.TryRemove(projectName, out var _);
    }

    public void Dispose()
    {
        _projects.Clear();
        _workspace.ClearSolution();
        _workspace.Dispose();
    }

    private readonly ConcurrentDictionary<string, Lazy<AsyncLock>> _projectAnalysisLock = new ConcurrentDictionary<string, Lazy<AsyncLock>>();

    public async Task AnalysisProjectAsync(string projectName, CancellationToken cancellationToken = default)
    {
        var @lock = _projectAnalysisLock.GetOrAdd(projectName, () => new Lazy<AsyncLock>()).Value;

        using (await @lock.LockAsync(cancellationToken))
        {
            var roslynProject = GetRoslynProject(projectName);
            var project = _workspace.CurrentSolution.GetProject(roslynProject.Id);

            var syntaxTrees = new List<SyntaxTree>();
            //   
            foreach (var document in project.Documents)
            {
                // 
                syntaxTrees.Add(await document.GetSyntaxTreeAsync(cancellationToken));

                // resolve document references
                var documentTxt = (await document.GetTextAsync(cancellationToken)).ToString();
                var documentMetaReferences = new List<MetadataReference>();
                var packageReferences = await _cSharpPackageResolver.ResolveAsync(documentTxt, cancellationToken);
                foreach (var packageReference in packageReferences)
                {
                    documentMetaReferences.AddRange(await packageReference.GetReferencesAsync(_serviceProvider, cancellationToken));
                }
                roslynProject.UpdateDocumentMetadataReferences(document.Name, documentMetaReferences);
            }

            var solution = _workspace.CurrentSolution.WithProjectMetadataReferences(roslynProject.Id, roslynProject.AllMetadataReferences);

            _workspace.TryApplyChanges(solution);
        }
    }

    public async Task<CSharpCompilation> GetCompilationAsync(string projectName, CancellationToken cancellationToken = default)
    {
        var roslynProject = GetRoslynProject(projectName);
        var project = _workspace.CurrentSolution.GetProject(roslynProject.Id);

        var syntaxTrees = new List<SyntaxTree>();
        //   
        foreach (var document in project.Documents)
        {
            // 
            syntaxTrees.Add(await document.GetSyntaxTreeAsync(cancellationToken));

            // resolve document references
            var documentTxt = (await document.GetTextAsync(cancellationToken)).ToString();
            var documentMetaReferences = new List<MetadataReference>();
            var packageReferences = await _cSharpPackageResolver.ResolveAsync(documentTxt, cancellationToken);
            foreach (var packageReference in packageReferences)
            {
                documentMetaReferences.AddRange(await packageReference.GetReferencesAsync(_serviceProvider, cancellationToken));
            }
            roslynProject.UpdateDocumentMetadataReferences(document.Name, documentMetaReferences);
        }

        var compilationOptions = CreateCompilationOptions(roslynProject.Imports);

        return CSharpCompilation.Create(
            project.Id.Id.ToString("N"),
            syntaxTrees: syntaxTrees,
            references: roslynProject.AllMetadataReferences,
            options: compilationOptions);
    }

    public async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string projectName, CancellationToken cancellationToken = default)
    {
        var roslynProject = GetRoslynProject(projectName);
        var compilation = await GetCompilationAsync(projectName, cancellationToken);

        var root = Path.Combine(Path.GetTempPath(), "workflow-elsa", "roslyns", roslynProject.Id.Id.ToString());
        if (!Directory.Exists(root))
            Directory.CreateDirectory(root);

        var file = Path.Combine(root, roslynProject.Id.Id.ToString());

        var emitResult = compilation.Emit(file + ".dll", file + ".pdb", cancellationToken: cancellationToken);

        if (emitResult.Success)
            return ImmutableArray<Diagnostic>.Empty;

        return emitResult.Diagnostics;
    }

    public async Task<ImmutableArray<Diagnostic>> GetDocumentDiagnosticsAsync(string projectName, string name, CancellationToken cancellationToken = default)
    {
        var roslynProject = GetRoslynProject(projectName);
        var project = _workspace.CurrentSolution.GetProject(roslynProject.Id);

        var compilation = await GetCompilationAsync(projectName, cancellationToken);

        var root = GetProjectFolder(roslynProject.Id);
        var file = Path.Combine(root, roslynProject.Id.Id.ToString());

        var emitResult = compilation.Emit(file + ".dll", file + ".pdb", cancellationToken: cancellationToken);

        if (emitResult.Success)
            return ImmutableArray<Diagnostic>.Empty;

        var result = new List<Diagnostic>();
        foreach (var diagnostic in emitResult.Diagnostics)
        {
            if (diagnostic.Location?.IsInSource != true)
            {
                continue;
            }

            var filePath = diagnostic.Location.SourceTree?.FilePath;

            if (Path.GetFileNameWithoutExtension(filePath) == name)
                result.Add(diagnostic);
        }

        return result.ToImmutableArray();
    }

    protected IEnumerable<Type> GetCompositionTypes() => DefaultCompositionTypes;

    protected ParseOptions GetParseOptions() => DefaultParseOptions;

    // protected IEnumerable<string> GetImports() => _defaultImports.Concat(_additionalImports ?? new string[0]);

    protected Project CreateProject(AdhocWorkspace workspace, string projectName, IEnumerable<Assembly> _additionalAssemblies, IEnumerable<string> _additionalImports)
    {
        var references = DefaultMetadataReferences.ToList();
        if (_additionalAssemblies != null)
            references.AddRange(_additionalAssemblies.Select(x => MetadataReference.CreateFromFile(x.Location)));

        var compilationOptions = CreateCompilationOptions(_additionalImports);

        // var analyzerReferences = GetAnalyzerReferences(_compositionHost.GetExport<IAnalyzerAssemblyLoader>());

        var projectId = ProjectId.CreateNewId();

        var root = GetProjectFolder(projectId);
        var file = Path.Combine(root, projectId.Id.ToString());

        var projectInfo = ProjectInfo.Create(
            projectId,
            VersionStamp.Create(),
            projectName,
            projectName,
            LanguageNames.CSharp,
            filePath: $"{file}.dll",
            outputFilePath: file,
            compilationOptions: compilationOptions,
            parseOptions: _parseOptions,
            metadataReferences: references
        // analyzerReferences: analyzerReferences
        );

        var project = workspace.AddProject(projectInfo);

        if (compilationOptions.Usings.Length > 0)
        {
            var globalUsingCode = string.Join("\n", compilationOptions.Usings.Select(x => $"global using {x};"));
            CreateDocument(project.Id, "GeneratedUsings", globalUsingCode);
        }

        return project;
    }

    protected CSharpCompilationOptions CreateCompilationOptions(IEnumerable<string> _additionalImports)
    {
        var hostImports = DefaultImports.Concat(_additionalImports ?? new string[0]);

        return new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            reportSuppressedDiagnostics: false,
            usings: hostImports,
            checkOverflow: true,
            allowUnsafe: false,
            generalDiagnosticOption: ReportDiagnostic.Warn,
            specificDiagnosticOptions: _specificDiagnosticOptions,
            concurrentBuild: false,
            metadataReferenceResolver: CSharpScriptMetadataResolver.Instance,
            nullableContextOptions: NullableContextOptions.Enable);
    }

    private static string GetProjectFolder(ProjectId projectId)
    {
        var root = Path.Combine(Path.GetTempPath(), "workflow-elsa", "roslyns", projectId.Id.ToString());
        if (!Directory.Exists(root))
            Directory.CreateDirectory(root);

        return root;
    }

    public TService GetService<TService>()
    {
        return _compositionHost.GetExport<TService>();
    }
}
