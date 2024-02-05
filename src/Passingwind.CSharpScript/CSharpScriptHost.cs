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

namespace Passingwind.CSharpScriptEngine;

/// <summary>
///
/// https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md
///
/// </summary>
public class CSharpScriptHost : ICSharpScriptHost, IDisposable
{
    public static ImmutableArray<string> PreprocessorSymbols => ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

    public static ImmutableArray<string> DefaultSuppressedDiagnostics => ImmutableArray.Create("CS1701", "CS1702", "CS1705", "CS7011", "CS8097");

    public static ParseOptions DefaultParseOptions => CSharpParseOptions
        .Default
        .WithPreprocessorSymbols(PreprocessorSymbols)
        .WithDocumentationMode(DocumentationMode.Parse)
        .WithLanguageVersion(LanguageVersion.Latest)
        .WithKind(SourceCodeKind.Script)
        .CommonWithKind(SourceCodeKind.Script);

#if NET8_0
    public static ImmutableArray<MetadataReference> StandardMetadataReference = ImmutableArray.CreateRange(Basic.Reference.Assemblies.Net80.References.All.Select(GetMetadataReference));
    public static ImmutableArray<PortableExecutableReference> StandardPortableReference = ImmutableArray.CreateRange(Basic.Reference.Assemblies.Net80.References.All);
#elif NET7_0
    public static ImmutableArray<MetadataReference> StandardMetadataReference = ImmutableArray.CreateRange(Basic.Reference.Assemblies.Net70.References.All.Select(GetMetadataReference));
    public static ImmutableArray<PortableExecutableReference> StandardPortableReference = ImmutableArray.CreateRange(Basic.Reference.Assemblies.Net70.References.All);
#elif NET6_0
    public static ImmutableArray<MetadataReference> StandardMetadataReference = ImmutableArray.CreateRange(Basic.Reference.Assemblies.Net60.References.All.Select(GetMetadataReference));
    public static ImmutableArray<PortableExecutableReference> StandardPortableReference = ImmutableArray.CreateRange(Basic.Reference.Assemblies.Net60.References.All);
#else
    public static ImmutableArray<MetadataReference> StandardMetadataReference = ImmutableArray.CreateRange(Basic.Reference.Assemblies.NetStandard20.References.All.Select(GetMetadataReference));
    public static ImmutableArray<PortableExecutableReference> StandardPortableReference = ImmutableArray.CreateRange(Basic.Reference.Assemblies.NetStandard20.References.All);
#endif

    public static ImmutableArray<MetadataReference> DefaultRuntimeMetadataReference = ImmutableArray.CreateRange<MetadataReference>(GetRuntimeAssembly().Select(x => MetadataReference.CreateFromFile(x.Location)));

    ///// <summary>
    ///// default references
    ///// </summary>
    //public static ImmutableArray<MetadataReference> DefaultReferences = new[] {
    //        Assembly.Load("System.Runtime"), // System.Runtime.dll
    //        Assembly.Load("netstandard") // netstandard.dll
    //    }.Select(x => MetadataReference.CreateFromFile(x.Location))
    //    .ToImmutableArray<MetadataReference>();

    /// <summary>
    /// default imports
    /// </summary>
    public static ImmutableArray<string> DefaultImports => new string[] {
            "System",
            "System.Collections",
            "System.Collections.Concurrent",
            "System.Collections.Generic",
            "System.Dynamic",
            "System.Globalization",
            "System.IO",
            "System.Linq",
            "System.Reflection",
            "System.Text",
            "System.Text.RegularExpressions",
            "System.Threading",
            "System.Threading.Tasks"
        }.ToImmutableArray();

    private static readonly ConcurrentDictionary<string, CSharpScriptContextState> ScriptStateCache = new ConcurrentDictionary<string, CSharpScriptContextState>();

    private readonly IScriptReferenceResolver _scriptReferenceResolver;

    public CSharpScriptHost(IScriptReferenceResolver scriptReferenceResolver)
    {
        _scriptReferenceResolver = scriptReferenceResolver;
    }

    public async Task<object?> RunAsync(CSharpScriptContext context, Action<ScriptOptions>? scriptOptions = null
        , CancellationToken cancellationToken = default)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        Script script;
        if (!string.IsNullOrWhiteSpace(context.ScriptId))
        {
            var scriptId = context.ScriptId;

            CSharpScriptContextState? currentState = null;

            bool changed = false;
            if (ScriptStateCache.TryGetValue(scriptId, out var previousContext))
            {
                changed = context.IsChanged(previousContext.Context);
                currentState = previousContext;
            }

            if (changed || currentState == null)
            {
                var options = CreateScriptOptions(context.Imports, context.Assemblies);
                scriptOptions?.Invoke(options);

                options = options.AddReferences(StandardMetadataReference);

                currentState = new CSharpScriptContextState(context)
                {
                    Script = await CreateScriptAsync(context.SourceText, options, context.EvaluationGlobal.GetType(), cancellationToken)
                };

                ScriptStateCache[scriptId] = currentState;
            }

            script = currentState.Script;
        }
        else
        {
            var options = CreateScriptOptions(context.Imports, context.Assemblies);
            scriptOptions?.Invoke(options);

            options = options.AddReferences(StandardMetadataReference);

            script = await CreateScriptAsync(context.SourceText, options, context.EvaluationGlobal.GetType(), cancellationToken);
        }

        try
        {
            var result = await script.RunAsync(context.EvaluationGlobal, cancellationToken: cancellationToken);

            return result?.ReturnValue;
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

    public async Task<EmitResult?> CompileAsync(CSharpScriptCompileContext context, CancellationToken cancellationToken = default)
    {
        var scriptOptions = CreateScriptOptions(context.Imports, context.Assemblies);
        var parseOptions = CreateParseOptions();

        scriptOptions = scriptOptions.AddReferences(StandardPortableReference);

        var syntaxTree = SyntaxFactory.ParseSyntaxTree(context.SourceText, options: parseOptions, encoding: Encoding.UTF8, cancellationToken: cancellationToken);

        // remove reference directive
        syntaxTree = syntaxTree.RemoveReferenceDirectives(parseOptions, cancellationToken: cancellationToken);

        var name = Guid.NewGuid().ToString("N");
        var rootFolder = Path.Combine(Path.GetTempPath(), "workflow", "csharp", name);

        if (!Directory.Exists(rootFolder))
        {
            Directory.CreateDirectory(rootFolder);
        }

        var compilationOptions = new CSharpCompilationOptions(
            outputKind: OutputKind.DynamicallyLinkedLibrary,
            scriptClassName: "Program",
            usings: scriptOptions.Imports,
            checkOverflow: scriptOptions.CheckOverflow,
            allowUnsafe: scriptOptions.AllowUnsafe,
            metadataReferenceResolver: scriptOptions.MetadataResolver,
            nullableContextOptions: NullableContextOptions.Enable);

        var compilation = CSharpCompilation.Create(
            name,
            syntaxTrees: new[] { syntaxTree },
            references: scriptOptions.MetadataReferences,
            options: compilationOptions);

        var file = Path.Combine(rootFolder, name);

        await using var peStream = File.OpenWrite($"{file}.dll");
        await using var pdbStream = File.OpenWrite($"{file}.pdb");

        return compilation.Emit(peStream, pdbStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb), cancellationToken: cancellationToken);
    }

    private async Task<Script<object>> CreateScriptAsync(string code, ScriptOptions options, Type globalType, CancellationToken cancellationToken = default)
    {
        var references = await _scriptReferenceResolver.ResolveReferencesAsync(code, cancellationToken);

        options = options.AddReferences(references);

        var loader = new InteractiveAssemblyLoader();

        return CSharpScript.Create(code, options: options, globalsType: globalType, assemblyLoader: loader);
    }

    public static ScriptOptions CreateScriptOptions(IEnumerable<string>? imports = null, IEnumerable<Assembly>? assemblies = null)
    {
        var parseOptions = CreateParseOptions();

        var options = ScriptOptions.Default
            .WithReferences(DefaultRuntimeMetadataReference)
            .AddImports(DefaultImports)
            .WithMetadataResolver(NuGetMetadataReferenceResolver.Instance)
            .WithSourceResolver(SourceFileResolver.Default)
            .WithAllowUnsafe(false)
            .WithCheckOverflow(true)
            .WithFileEncoding(Encoding.UTF8)
#if DEBUG
            .WithOptimizationLevel(OptimizationLevel.Debug)
#else
            .WithOptimizationLevel(OptimizationLevel.Release)
#endif
            ;

        if (imports != null)
        {
            options = options.AddImports(imports);
        }

        if (assemblies != null)
        {
            options = options.AddReferences(assemblies);
        }

        return options;
    }

    public static ParseOptions CreateParseOptions()
    {
        return DefaultParseOptions;
    }

    private static MetadataReference GetMetadataReference(PortableExecutableReference reference)
    {
        return MetadataReference.CreateFromFile(Assembly.Load(Path.GetFileNameWithoutExtension(reference.FilePath)!).Location);
    }

    private static IEnumerable<Assembly> GetRuntimeAssembly()
    {
        return new[] { Assembly.Load("System.Runtime"), Assembly.Load("netstandard") };
    }

    public void Dispose()
    {
        ScriptStateCache.Clear();
    }
}
