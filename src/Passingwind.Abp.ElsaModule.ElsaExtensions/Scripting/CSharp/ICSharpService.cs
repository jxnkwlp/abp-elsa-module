using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public interface ICSharpService : IScopedDependency
{
    Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext context, Action<CSharpEvaluationGlobal> configure, CancellationToken cancellationToken = default);
}
