using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Expressions;
using Elsa.Services.Models;
using Microsoft.Extensions.Options;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Expressions;

public class CSharpExpressionHandler : IExpressionHandler
{
    public string Syntax => CSharpSyntaxName.CSharp;

    private readonly ICSharpService _cSharpService;
    private readonly IOptions<CSharpScriptOptions> _options;

    public CSharpExpressionHandler(ICSharpService cSharpService, IOptions<CSharpScriptOptions> options)
    {
        _cSharpService = cSharpService;
        _options = options;
    }

    public async Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext context, CancellationToken cancellationToken)
    {
        return await _cSharpService.EvaluateAsync(expression, returnType, context, (g) => { }, cancellationToken);
    }
}
