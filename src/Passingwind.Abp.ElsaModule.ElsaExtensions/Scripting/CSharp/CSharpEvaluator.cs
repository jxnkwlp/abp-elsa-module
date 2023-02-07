using System;
using System.Collections.Generic;
using System.Linq;
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

        var scriptOptions = CreateOptions(evaluationContext.ScriptOptions.Imports, evaluationContext.ScriptOptions.Assemblies);

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

        var scriptOptions = CreateOptions(evaluationContext.ScriptOptions.Imports, evaluationContext.ScriptOptions.Assemblies);

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

    private static ScriptOptions CreateOptions(IEnumerable<string> imports, IEnumerable<Assembly> assemblies)
    {
        // default imports
        var defaultImports = new string[] {
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
        };

        // default references
        var defaultReferences = new[] {
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
        };

        var options = ScriptOptions.Default
            .AddReferences(defaultReferences.Concat(assemblies))
            .AddImports(defaultImports.Concat(imports));

        return options;
    }
}
