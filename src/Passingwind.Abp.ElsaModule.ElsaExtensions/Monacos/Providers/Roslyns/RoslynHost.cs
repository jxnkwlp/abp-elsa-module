using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;

public class RoslynHost : IDisposable
{
    private const string _scriptName = "Program";

    private static readonly ImmutableArray<string> _preprocessorSymbols = ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

    private static readonly ImmutableArray<Assembly> _defaultCompositionAssemblies =
        ImmutableArray.Create(
            Assembly.Load("Microsoft.CodeAnalysis"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp.Workspaces"),
            Assembly.Load("Microsoft.CodeAnalysis.Features"),
            Assembly.Load("Microsoft.CodeAnalysis.Workspaces"),
            typeof(RoslynHost).Assembly);

    private static readonly ImmutableArray<Type> _defaultCompositionTypes =
        _defaultCompositionAssemblies.SelectMany(t => t.GetTypes())
        .ToImmutableArray();

    private static readonly ParseOptions _parseOptions = CSharpParseOptions
        .Default
        .WithPreprocessorSymbols(_preprocessorSymbols)
        .WithKind(SourceCodeKind.Script);

    //private readonly IDocumentationProviderService _documentationProviderService; 
    private readonly RoslynHostReference _hostReference;
    private CompositionHost _compositionContext;
    private CSharpCompilationOptions _compilationOptions;
    private ProjectId _projectId;

    public AdhocWorkspace Workspace { get; private set; }
    public Project Project => Workspace.CurrentSolution?.GetProject(_projectId);

    public RoslynHost(RoslynHostReference hostReference)
    {
        _hostReference = hostReference;
        // 
        Initial();
    }

    protected void Initial()
    {
        _compilationOptions = CreateCompilationOptions();

        var partTypes = _defaultCompositionTypes.ToArray();

        if (_hostReference.Assemblies?.Any() == true)
        {
            partTypes = partTypes.Concat(_hostReference.Assemblies.SelectMany(x => x.GetTypes())).Distinct().ToArray();
        }

        _compositionContext = new ContainerConfiguration()
                   .WithParts(partTypes.Distinct())
                   .CreateContainer();

        var host = MefHostServices.Create(_compositionContext);

        Workspace = new AdhocWorkspace(host);

        var name = Guid.NewGuid().ToString("N");

        _projectId = CreateDefaultProject(name).Id;

        if (_compilationOptions.Usings.Length > 0)
        {
            var globalUsingCode = string.Join("\n", _compilationOptions.Usings.Select(x => $"global using {x};"));
            AddOrUpdateDocument("GeneratedUsings", globalUsingCode);
        }

        // add empty file
        AddOrUpdateScriptDocument("");
    }

    public void AddOrUpdateScriptDocument(string sourceText)
    {
        AddOrUpdateDocument(_scriptName, sourceText);
    }

    public Document GetScriptDocument()
    {
        return Project!.Documents.FirstOrDefault(x => x.Name == $"{_scriptName}.cs");
    }

    public Document AddOrUpdateDocument(string name, string sourceText)
    {
        var doc = Project!.Documents.FirstOrDefault(x => x.Name == $"{name}.cs");
        if (doc != null)
        {
            doc = doc.WithText(SourceText.From(sourceText));

            UpdateDocument(doc);

            return doc;
        }
        else
        {
            return Workspace.AddDocument(Project!.Id, $"{name}.cs", SourceText.From(sourceText));
        }
    }

    public void RemoveDocument(string name)
    {
        var doc = Project!.Documents.FirstOrDefault(x => x.Name == $"{_scriptName}.cs");
        if (doc != null) _ = Workspace.CurrentSolution.RemoveDocument(doc.Id);
    }

    public Document GetDocument(string name)
    {
        return Project!.Documents.FirstOrDefault(x => x.Name == $"{name}.cs" || (name.EndsWith(".cs") && x.Name == name));
    }

    public void UpdateDocument(Document document)
    {
        Workspace.TryApplyChanges(document.Project.Solution);
    }

    public bool IsScriptDocument(Document document)
    {
        return document.Name == $"{_scriptName}.cs";
    }

    public void AddImports(params string[] imports)
    {
        _compilationOptions = _compilationOptions.WithUsings(_compilationOptions.Usings.Concat(imports));
    }

    public async Task<CSharpCompilation> GetCompilationAsync()
    {
        var syntaxTrees = new List<SyntaxTree>();
        //   
        foreach (var document in Project.Documents)
        {
            syntaxTrees.Add(await document.GetSyntaxTreeAsync());
        }

        var name = Guid.NewGuid().ToString("N");
        return CSharpCompilation.Create(
            name,
            syntaxTrees: syntaxTrees,
            options: _compilationOptions,
            references: Project.MetadataReferences);
    }

    private Project CreateDefaultProject(string projectName)
    {
        var hostRefs = _hostReference.GetReferences();

        var project = ProjectInfo.Create(
            ProjectId.CreateNewId(),
            VersionStamp.Create(),
            projectName,
            projectName,
            LanguageNames.CSharp,
            parseOptions: _parseOptions,
            compilationOptions: _compilationOptions,
            metadataReferences: hostRefs
        );

        return Workspace.AddProject(project);
    }

    private CSharpCompilationOptions CreateCompilationOptions()
    {
        var hostImports = _hostReference.GetImports();

        return new CSharpCompilationOptions(
            OutputKind.ConsoleApplication,
            scriptClassName: "Program",
            allowUnsafe: false,
            checkOverflow: true,
            concurrentBuild: false,
            usings: hostImports,
            nullableContextOptions: NullableContextOptions.Enable);
    }

    public void Dispose()
    {
        Workspace?.Dispose();
    }
}
