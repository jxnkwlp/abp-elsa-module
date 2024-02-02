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
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
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

    public static ParseOptions DefaultParseOptions => CSharpParseOptions
        .Default
        .WithPreprocessorSymbols(PreprocessorSymbols)
        .WithKind(SourceCodeKind.Script)
        .CommonWithKind(SourceCodeKind.Regular);

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
            return new CSharpScriptProject(_workspace, csProject);
        });
    }

    protected Project CreateProject(AdhocWorkspace workspace, string projectName, CSharpCompilationOptions compilationOptions)
    {
        var references = DefaultReferences.ToList();

        var analyzerReferences = GetSolutionAnalyzerReferences();

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
            parseOptions: DefaultParseOptions,
            metadataReferences: references,
            analyzerReferences: analyzerReferences
        );

        return workspace.AddProject(projectInfo);

        //if (compilationOptions.Usings.Length > 0)
        //{
        //    var globalUsingCode = string.Join("\n", compilationOptions.Usings.Select(x => $"global using {x};"));
        //    CreateDocument(project.Id, "GeneratedUsings", globalUsingCode);
        //} 
    }

    protected CSharpCompilationOptions CreateDefaultCompilationOptions(IEnumerable<string>? AdditionalImports = null)
    {
        var hostImports = DefaultImports.Concat(AdditionalImports ?? Array.Empty<string>());

        return new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            reportSuppressedDiagnostics: false,
            usings: hostImports,
            checkOverflow: true,
            allowUnsafe: false,
            generalDiagnosticOption: ReportDiagnostic.Warn,
            specificDiagnosticOptions: _specificDiagnosticOptions,
            concurrentBuild: false,
            metadataReferenceResolver: CSharpScriptMetadataReferenceResolver.Instance,
            nullableContextOptions: NullableContextOptions.Enable);
    }

    protected virtual IEnumerable<AnalyzerReference> GetSolutionAnalyzerReferences()
    {
        var loader = GetService<IAnalyzerAssemblyLoader>();
        yield return new AnalyzerFileReference(typeof(Compilation).Assembly.Location, loader);
        yield return new AnalyzerFileReference(typeof(CSharpResources).Assembly.Location, loader);
        yield return new AnalyzerFileReference(typeof(FeaturesResources).Assembly.Location, loader);
        yield return new AnalyzerFileReference(typeof(CSharpFeaturesResources).Assembly.Location, loader);
    }

    private static string GetProjectFolder(ProjectId projectId)
    {
        var root = Path.Combine(Path.GetTempPath(), "roslyns", projectId.Id.ToString());
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

    public async Task<CSharpCompilation> CreateCompilationAsync(ICSharpScriptProject scriptProject, CancellationToken cancellationToken = default)
    {
        var scriptOptions = CSharpScriptHost.CreateScriptOptions(scriptProject.GetImports(), scriptProject.GetReferences());
        var parseOptions = CSharpScriptHost.CreateParseOptions();

        scriptOptions = scriptOptions.AddReferences(CSharpScriptHost.StandardPortableReference);

        var syntaxTrees = new List<SyntaxTree>();

        foreach (var item in scriptProject.Project.Documents)
        {
            var sourceText = (await item.GetTextAsync()).ToString();

            scriptOptions.AddReferences(await _scriptReferenceResolver.ResolveReferencesAsync(sourceText));

            var syntaxTree = SyntaxFactory.ParseSyntaxTree(sourceText, options: parseOptions, encoding: Encoding.UTF8, cancellationToken: cancellationToken);

            // remove reference directive
            syntaxTree = syntaxTree.RemoveReferenceDirectives(parseOptions, cancellationToken: cancellationToken);

            syntaxTrees.Add(syntaxTree);
        }

        var name = scriptProject.Name;

        var compilationOptions = new CSharpCompilationOptions(
            outputKind: OutputKind.DynamicallyLinkedLibrary,
            scriptClassName: "Program",
            usings: scriptOptions.Imports,
            checkOverflow: scriptOptions.CheckOverflow,
            allowUnsafe: scriptOptions.AllowUnsafe,
            metadataReferenceResolver: scriptOptions.MetadataResolver,
            nullableContextOptions: NullableContextOptions.Enable);

        return CSharpCompilation.Create(
            name,
            syntaxTrees: syntaxTrees,
            references: scriptOptions.MetadataReferences,
            options: compilationOptions);
    }

    public async Task<EmitResult> CompileAsync(ICSharpScriptProject scriptProject, CancellationToken cancellationToken = default)
    {
        var name = scriptProject.Name;

        var compilation = await CreateCompilationAsync(scriptProject, cancellationToken);

        var rootFolder = Path.Combine(Path.GetTempPath(), "workflow", "csharp", name);

        if (!Directory.Exists(rootFolder))
        {
            Directory.CreateDirectory(rootFolder);
        }

        var file = Path.Combine(rootFolder, name);

        await using var peStream = File.OpenWrite($"{file}.dll");
        await using var pdbStream = File.OpenWrite($"{file}.pdb");

        return compilation.Emit(peStream, pdbStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb), cancellationToken: cancellationToken);
    }
}
