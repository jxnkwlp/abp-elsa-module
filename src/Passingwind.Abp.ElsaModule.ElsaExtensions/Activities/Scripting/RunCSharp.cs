using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Metadata;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Options;
using Passingwind.Abp.ElsaModule.Services;

namespace Passingwind.Abp.ElsaModule.Activities.Scripting
{
    [Action(
        DisplayName = "Run CSharp (experiment)",
        Category = "Scripting",
        Description = "Run CSharp code.",
        Outcomes = new[] { OutcomeNames.Done })]
    public class RunCSharp : Activity, IActivityPropertyOptionsProvider
    {
        [ActivityInput(Hint = "The CSharp code to run.", UIHint = ActivityInputUIHints.CodeEditor, OptionsProvider = typeof(RunCSharp))]
        public string Script { get; set; }

        [ActivityInput(
            Hint = "The possible outcomes that can be set by the script.",
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = SyntaxNames.Json,
            SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid },
            ConsiderValuesAsOutcomes = true
        )]
        public ICollection<string> PossibleOutcomes { get; set; } = new List<string> { OutcomeNames.Done };

        [ActivityOutput] public object Output { get; set; }


        private readonly ICSharpEvaluator _iCSharpEvaluator;
        private readonly IOptions<CSharpOptions> _options;

        public RunCSharp(ICSharpEvaluator iCSharpEvaluator, IOptions<CSharpOptions> options)
        {
            _iCSharpEvaluator = iCSharpEvaluator;
            _options = options;
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var script = Script;

            if (string.IsNullOrWhiteSpace(script))
                return Done();

            var outcomes = new List<string>();

            var cSharpEvaluationContext = new CSharpEvaluationContext { Imports = _options.Value.Imports, };

            var setOutcome = (string value) => outcomes.Add(value);
            var setOutcomes = (IEnumerable<string> values) => outcomes.AddRange(values);

            var output = await _iCSharpEvaluator.EvaluateAsync(script, typeof(object), cSharpEvaluationContext, context, (g) =>
            {
                g.Dynamic.SetOutcome = setOutcome;
                g.Dynamic.SetOutcomes = setOutcomes;
            }, context.CancellationToken);

            if (!outcomes.Any())
                outcomes.Add(OutcomeNames.Done);

            Output = output;

            return Outcomes(outcomes);
        }

        public object GetOptions(PropertyInfo property)
        {
            if (property.Name != nameof(Script))
                return null;

            return new
            {
                EditorHeight = "Large",
                Context = nameof(RunCSharp),
                Syntax = "C#",
            };
        }

    }
}
