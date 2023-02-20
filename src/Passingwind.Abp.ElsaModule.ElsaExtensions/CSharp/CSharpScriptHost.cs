using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace Passingwind.Abp.ElsaModule.CSharp;

// 
// https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md
// 
public class CSharpScriptHost : ICSharpScriptHost
{
    // default references
    public static ImmutableArray<MetadataReference> DefaultReferences = new[] {
            Assembly.Load("System.Runtime"), // System.Runtime.dll
            Assembly.Load("netstandard"), // netstandard.dll
            typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly,  // Microsoft.CSharp
            typeof(System.Object).Assembly, // System.Private.CoreLib.dll
            typeof(System.Console).Assembly,  // System.Console.dll 
            typeof(System.Collections.Hashtable).Assembly,
            typeof(System.Collections.Generic.List<>).Assembly,
            typeof(System.ComponentModel.DescriptionAttribute).Assembly, // System.ComponentModel.Primitives.dll
            typeof(System.Data.DataSet).Assembly, // System.Data.Common.dll
            typeof(System.Xml.XmlDocument).Assembly, // System.Private.Xml.dll
            typeof(System.ComponentModel.INotifyPropertyChanged).Assembly, // System.ObjectModel.dll
            typeof(System.Linq.Enumerable).Assembly, // System.Linq.dll
            typeof(System.Linq.Expressions.Expression).Assembly, // System.Linq.Expressions.dll
        }.Select(x => MetadataReference.CreateFromFile(x.Location))
        // TODO
        //.Concat(Basic.Reference.Assemblies.Net60.References.All)
        .ToImmutableArray<MetadataReference>();

    // default imports
    public static ImmutableArray<string> DefaultImports => new string[] {
            "System",
            "System.Console",
            "System.Collections",
            "System.Collections.Generic",
            "System.Collections.Concurrent",
            "System.Dynamic",
            "System.Linq",
            "System.Globalization",
            "System.IO",
            "System.Text",
            "System.Text.Encoding",
            "System.Text.RegularExpressions",
            "System.Threading",
            "System.Threading.Tasks",
            "System.Threading.Tasks.Parallel",
            "System.Threading.Thread",
            "System.ValueTuple",
            "System.Reflection",
        }.ToImmutableArray();

    private static readonly ConcurrentDictionary<string, CSharpScriptContextState> _scriptStateCache = new ConcurrentDictionary<string, CSharpScriptContextState>();

    private readonly ICSharpPackageResolver _cSharpPackageResolver;
    private readonly IServiceProvider _serviceProvider;

    public CSharpScriptHost(ICSharpPackageResolver cSharpPackageResolver, IServiceProvider serviceProvider)
    {
        _cSharpPackageResolver = cSharpPackageResolver;
        _serviceProvider = serviceProvider;
    }

    public async Task<object> RunWithIdAsync(string scriptId, CSharpScriptContext context, Action<ScriptOptions> optionsConfigure, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (string.IsNullOrWhiteSpace(scriptId)) throw new ArgumentException($"'{nameof(scriptId)}' cannot be null or whitespace.", nameof(scriptId));

        CSharpScriptContextState scriptState = null;

        bool changed = false;
        if (_scriptStateCache.TryGetValue(scriptId, out var previousContext))
        {
            changed = context.IsChanged(previousContext.Context);
            scriptState = previousContext;
        }

        if (changed || scriptState == null)
        {
            var scriptOptions = CreateOptions(context.Imports, context.Assemblies);
            optionsConfigure?.Invoke(scriptOptions);

            scriptState = new CSharpScriptContextState(context)
            {
                Script = await CreateScriptAsync(context.SourceText, scriptOptions, context.EvaluationGlobal.GetType())
            };

            _scriptStateCache[scriptId] = scriptState;
        }

        // TODO: clear old state
        // 

        try
        {
            var result = await scriptState.Script.RunAsync(context.EvaluationGlobal, cancellationToken: cancellationToken);

            return result?.ReturnValue ?? null;
        }
        catch (CompilationErrorException e)
        {
            throw new Exception($"Compiler failed. \n{string.Join(Environment.NewLine, e.Diagnostics)}");
        }
        catch (Exception e)
        {
            throw new Exception("Run csharp code failed.", innerException: e);
        }
    }

    public async Task<object> RunAsync(CSharpScriptContext context, Action<ScriptOptions> optionsConfigure, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var scriptOptions = CreateOptions(context.Imports, context.Assemblies);
        optionsConfigure?.Invoke(scriptOptions);

        try
        {
            var script = await CreateScriptAsync(context.SourceText, scriptOptions, context.EvaluationGlobal.GetType());

            var result = await script.RunAsync(context.EvaluationGlobal, cancellationToken: cancellationToken);

            return result?.ReturnValue ?? null;
        }
        catch (CompilationErrorException e)
        {
            throw new Exception($"Compiler failed. \n{string.Join(Environment.NewLine, e.Diagnostics)}");
        }
        catch (Exception e)
        {
            throw new Exception("Run csharp code failed.", innerException: e);
        }
    }

    public void Compile(CSharpScriptContext context, CancellationToken cancellationToken = default)
    {
        var scriptOptions = CreateOptions(context.Imports, context.Assemblies);
        var parseOptions = CreateParseOptions();

        var syntaxTree = SyntaxFactory.ParseSyntaxTree(context.SourceText, options: parseOptions, encoding: Encoding.UTF8, cancellationToken: cancellationToken);
        var syntaxRoot = syntaxTree.GetRoot();

        // remove reference directive
        var nodeRemove = syntaxTree.GetCompilationUnitRoot().GetReferenceDirectives();

        syntaxRoot = syntaxRoot.RemoveNodes(nodeRemove, SyntaxRemoveOptions.KeepExteriorTrivia);

        // 
        syntaxTree = syntaxTree.WithRootAndOptions(syntaxRoot, parseOptions);

        var name = Guid.NewGuid().ToString("N");
        var rootFolder = Path.Combine(Path.GetTempPath(), "workflow", "csharp", name);

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            scriptClassName: "Program",
            usings: scriptOptions.Imports,
            checkOverflow: scriptOptions.CheckOverflow,
            allowUnsafe: scriptOptions.AllowUnsafe,
            metadataReferenceResolver: scriptOptions.MetadataResolver,
            nullableContextOptions: NullableContextOptions.Disable);

        var compilation = CSharpCompilation.Create(
            name,
            syntaxTrees: new[] { syntaxTree },
            options: compilationOptions,
            references: scriptOptions.MetadataReferences);

        var file = Path.Combine(rootFolder, name);

        using var peStream = File.OpenWrite($"{file}.dll");
        using var pdbStream = File.OpenWrite($"{file}.pdb");

        var emitResult = compilation.Emit(peStream, pdbStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb), cancellationToken: cancellationToken);

        if (emitResult?.Success != true && !emitResult.Diagnostics.IsEmpty)
        {
            throw new AggregateException(emitResult.Diagnostics.Select(x => new Exception(x.ToString())));
        }
        else
        {
            // TODO: logging
        }
    }

    private async Task<Script<object>> CreateScriptAsync(string code, ScriptOptions options, Type globalType, CancellationToken cancellationToken = default)
    {
        var othersReferences = new List<MetadataReference>();

        var packages = await _cSharpPackageResolver.ResolveAsync(code, cancellationToken);

        foreach (var item in packages)
        {
            if (item is NuGetPackageReference nuGetPackage)
            {
                othersReferences.AddRange(await nuGetPackage.GetReferencesAsync(_serviceProvider, cancellationToken));
            }
        }

        options = options.AddReferences(othersReferences);

        var loader = new InteractiveAssemblyLoader();

        return CSharpScript.Create(code, options: options, globalsType: globalType, assemblyLoader: loader);
    }

    private static ScriptOptions CreateOptions(IEnumerable<string> imports, IEnumerable<Assembly> assemblies)
    {
        var parseOptions = CreateParseOptions();

        var options = ScriptOptions.Default
            .AddReferences(DefaultReferences)
            .AddImports(DefaultImports)
            .WithMetadataResolver(CSharpScriptMetadataResolver.Instance)
            .WithSourceResolver(SourceFileResolver.Default)
            .WithAllowUnsafe(false)
            .WithCheckOverflow(true)
            .WithFileEncoding(Encoding.UTF8)
#if DEBUG
            .WithOptimizationLevel(OptimizationLevel.Debug)
#else
            .WithOptimizationLevel(OptimizationLevel.Release)
#endif
            .WithParseOptions(parseOptions)
            ;

        if (imports != null)
            options = options.AddImports(imports);

        if (assemblies != null)
            options = options.AddReferences(assemblies);

        return options;
    }

    private static ParseOptions CreateParseOptions()
    {
        return CSharpParseOptions.Default.WithKind(SourceCodeKind.Script);
    }
}

public class CSharpScriptContextState
{
    public CSharpScriptContext Context { get; }

    public CSharpScriptContextState(CSharpScriptContext context)
    {
        Context = context;
    }

    public Script Script { get; set; }
}