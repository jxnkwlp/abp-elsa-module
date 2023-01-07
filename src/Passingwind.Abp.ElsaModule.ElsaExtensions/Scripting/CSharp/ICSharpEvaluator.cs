using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public interface ICSharpEvaluator : ITransientDependency
{
    Task<object> EvaluateAsync(string text, Type returnType, CSharpEvaluationContext evaluationContext, Action<ScriptOptions> optionsConfigure, CancellationToken cancellationToken = default);

    Task TestAsync(string text, CSharpEvaluationContext evaluationContext, Action<ScriptOptions> optionsConfigure, CancellationToken cancellationToken = default);
}