using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

// https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpEvaluator : ICSharpEvaluator
{
    private readonly IMediator _mediator;

    public CSharpEvaluator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<object> EvaluateAsync(string text, Type returnType, CSharpEvaluationContext evaluationContext, Action<ScriptOptions> optionsConfigure, CancellationToken cancellationToken = default)
    {
        string programText = text;

        var scriptOptions = CreateDefaultOptions();

        optionsConfigure?.Invoke(scriptOptions);

        await _mediator.Publish(new CSharpEvaluationConfigureNotification(programText, evaluationContext, scriptOptions));

        try
        {
            var script = CSharpScript.Create(programText, options: scriptOptions, globalsType: evaluationContext.EvaluationGlobal.GetType());

            var result = await script.RunAsync(evaluationContext.EvaluationGlobal, cancellationToken: cancellationToken);

            return result?.ReturnValue ?? null;
        }
        catch (CompilationErrorException e)
        {
            throw new Exception($"Compilation failed. \n{string.Join(Environment.NewLine, e.Diagnostics)}");
        }
        catch (Exception e)
        {
            throw new Exception("Run csharp code failed.", innerException: e);
        }
    }

    public Task TestAsync(string text, CSharpEvaluationContext evaluationContext, Action<ScriptOptions> optionsConfigure, CancellationToken cancellationToken = default)
    {
        string programText = text;

        var scriptOptions = CreateDefaultOptions();

        optionsConfigure?.Invoke(scriptOptions);

        try
        {
            var script = CSharpScript.Create(programText, options: scriptOptions, globalsType: evaluationContext.EvaluationGlobal.GetType());

            return Task.CompletedTask;
        }
        catch (CompilationErrorException e)
        {
            throw new Exception($"Compilation failed. \n{string.Join(Environment.NewLine, e.Diagnostics)}");
        }
        catch (Exception e)
        {
            throw new Exception("Run csharp code failed.", innerException: e);
        }
    }

    private static ScriptOptions CreateDefaultOptions()
    {
        // default imports
        var imports = new string[] {
            "System",
            "System.Dynamic",
            "System.Collections",
            "System.Collections.Concurrent",
            "System.Linq",
            "System.Console",
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
        };

        // add default references
        var options = ScriptOptions.Default
            .AddReferences(
                typeof(System.Object).GetTypeInfo().Assembly,  // Add reference to mscorlib
                typeof(System.Linq.Enumerable).GetTypeInfo().Assembly, // System.Linq
                typeof(System.Dynamic.DynamicObject).Assembly,  // System.Code
                typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly,  // Microsoft.CSharp
                typeof(System.Dynamic.ExpandoObject).Assembly  // System.Dynamic 
            )
            .AddImports(imports)
            ;

        return options;
    }
}
