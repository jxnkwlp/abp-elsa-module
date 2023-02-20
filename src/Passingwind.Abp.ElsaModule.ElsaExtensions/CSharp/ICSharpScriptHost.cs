using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public interface ICSharpScriptHost : ITransientDependency
{
    void Compile(CSharpScriptContext context, CancellationToken cancellationToken = default);
    Task<object> RunAsync(CSharpScriptContext context, Action<ScriptOptions> optionsConfigure, CancellationToken cancellationToken = default);
    Task<object> RunWithIdAsync(string scriptId, CSharpScriptContext context, Action<ScriptOptions> optionsConfigure, CancellationToken cancellationToken = default);
}