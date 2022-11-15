using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Expressions;
using Elsa.Services.Models;
using Microsoft.Extensions.Options;
using Passingwind.Abp.ElsaModule.Services;

namespace Passingwind.Abp.ElsaModule.Expressions
{
    public class CSharpHander : IExpressionHandler
    {
        public string Syntax => "C#";

        private readonly ICSharpEvaluator _iCSharpEvaluator;
        private readonly IOptions<CSharpOptions> _options;

        public CSharpHander(ICSharpEvaluator iCSharpEvaluator, IOptions<CSharpOptions> options)
        {
            _iCSharpEvaluator = iCSharpEvaluator;
            _options = options;
        }

        public async Task<object> EvaluateAsync(string expression, Type returnType, ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            var cSharpEvaluationContext = new CSharpEvaluationContext { Imports = _options.Value.Imports, };

            return await _iCSharpEvaluator.EvaluateAsync(expression, returnType, cSharpEvaluationContext, context, (c) => { }, cancellationToken);
        }
    }
}
