using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Options;

// https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md

namespace Passingwind.Abp.ElsaModule.Services
{
    public class CSharpEvaluator : ICSharpEvaluator
    {
        private readonly IMediator _mediator;
        private readonly IOptions<CSharpScriptOptions> _options;

        public CSharpEvaluator(IMediator mediator, IOptions<CSharpScriptOptions> options)
        {
            _mediator = mediator;
            _options = options;
        }

        public async Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext executionContext, Action<CSharpEvaluationContext> configure, CancellationToken cancellationToken = default)
        {
            string programText = expression;


            var engineOptions = CreateDefaultOptions();
            var context = new CSharpEvaluationContext(_options.Value, engineOptions, );

            var notification = new CSharpEvaluationContextConfigureEventNotification();
            await _mediator.Publish(notification);

            var global = new CSharpEvaluationGlobal();

            foreach (var item in notification.GlobalValues)
            {
                global.Context[item.Key] = item.Value;
            }

            try
            {


                var script =   CSharpScript.Create(programText, context.EngineOptions, context.EvaluationGlobal.GetType());

                script.RunAsync(context.EvaluationGlobal);

                var result = await CSharpScript.RunAsync(programText, context.EngineOptions, globals: context.EvaluationGlobal, cancellationToken: cancellationToken);
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
}
