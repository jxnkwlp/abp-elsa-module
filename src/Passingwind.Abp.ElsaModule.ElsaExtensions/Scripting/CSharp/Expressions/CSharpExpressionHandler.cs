using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Expressions;
using Elsa.Services.Models;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Expressions;

public class CSharpExpressionHandler : IExpressionHandler
{
    public string Syntax => CSharpSyntaxName.CSharp;

    private readonly ICSharpService _cSharpService;

    public CSharpExpressionHandler(ICSharpService cSharpService)
    {
        _cSharpService = cSharpService;
    }

    public async Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext context, CancellationToken cancellationToken)
    {
        return await _cSharpService.EvaluateAsync(expression, returnType, context, cancellationToken: cancellationToken);
    }
}
