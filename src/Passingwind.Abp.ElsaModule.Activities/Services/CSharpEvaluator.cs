using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

// https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md

namespace Passingwind.Abp.ElsaModule.Services
{
    public class CSharpEvaluator : ICSharpEvaluator
    {
        private readonly IMediator _mediator;

        public CSharpEvaluator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<object> EvaluateAsync(string expression, Type returnType, CSharpEvaluationContext evaluationContext, ActivityExecutionContext executionContext, Action<CSharpEvaluationGlobal> globalConfigure, CancellationToken cancellationToken = default)
        {
            var options = CreateDefaultOptions();

            if (evaluationContext.Imports?.Any() == true)
                options.AddImports(evaluationContext.Imports);

            string programText = expression;

            var input = new CSharpEvaluationGlobal
            {
                Context = executionContext
            };

            await _mediator.Publish(new CSharpEvaluattionEvent() { EvaluationContext = evaluationContext, ProgramText = programText });

            try
            {
                var result = await CSharpScript.RunAsync(programText, options, globals: input, cancellationToken: cancellationToken);
                return result?.ReturnValue;
            }
            catch (CompilationErrorException e)
            {
                throw new Exception($"Compilation failed. \n{string.Join(Environment.NewLine, e.Diagnostics)}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Microsoft.CodeAnalysis.Scripting.ScriptOptions CreateDefaultOptions()
        {
            var imports = new string[] {
                    "System",
                    "System.Dynamic",
                    "System.Collections",
                    "System.Collections.Concurrent",
                    "System.Linq",
                    "System.Console",
                    "System.Globalization",
                    "System.IO",
                    "System.Text.Encoding",
                    "System.Text.RegularExpressions",
                    "System.Threading",
                    "System.Threading.Tasks",
                    "System.Threading.Tasks.Parallel",
                    "System.Threading.Thread",
                    "System.ValueTuple",
            };

            // TODO
            var options = ScriptOptions.Default
                .AddReferences(
                    typeof(object).GetTypeInfo().Assembly,  // Add reference to mscorlib
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

    public class CSharpEvaluationGlobal
    {
        public ActivityExecutionContext Context { get; set; }

        public object GetVariable(string name) => Context.GetVariable(name);

        public void SetVariable(string name, object value) => Context.SetVariable(name, value);

        public dynamic Dynamic { get; set; }
    }

    public class CSharpEvaluationContext
    {
        public string[] Imports { get; set; }
    }

    public class CSharpEvaluattionEvent : INotification
    {
        public CSharpEvaluationContext EvaluationContext { get; set; }

        public string ProgramText { get; set; }
    }
}
