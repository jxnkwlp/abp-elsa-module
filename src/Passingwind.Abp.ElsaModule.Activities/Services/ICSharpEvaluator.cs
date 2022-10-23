using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;

namespace Passingwind.Abp.ElsaModule.Services
{
    public interface ICSharpEvaluator
    {
        Task<object> EvaluateAsync(string expression, Type returnType, CSharpEvaluationContext evaluationContext, ActivityExecutionContext executionContext, Action<CSharpEvaluationGlobal> globalConfigure, CancellationToken cancellationToken = default);
    }
}
