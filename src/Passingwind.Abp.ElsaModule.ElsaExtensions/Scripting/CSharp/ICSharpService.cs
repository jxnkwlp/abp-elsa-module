using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;
using Passingwind.Abp.ElsaModule.CSharp;
using Passingwind.CSharpScriptEngine;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public interface ICSharpService : IScopedDependency
{
    Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext context, Action<CSharpScriptEvaluationGlobal> globalConfigure = null, CancellationToken cancellationToken = default);
}
