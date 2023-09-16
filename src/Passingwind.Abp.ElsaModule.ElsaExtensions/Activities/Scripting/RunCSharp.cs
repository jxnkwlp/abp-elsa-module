using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Metadata;
using Elsa.Services;
using Elsa.Services.Models;
using MediatR;
using Passingwind.Abp.ElsaModule.Scripting.CSharp;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Activities.Scripting;

[Action(
    DisplayName = "Run CSharp",
    Category = "Scripting",
    Description = "Run CSharp code.",
    Outcomes = new[] { OutcomeNames.Done })]
public class RunCSharp : Activity, IActivityPropertyOptionsProvider, INotificationHandler<CSharpTypeDefinitionNotification>
{
    [ActivityInput(Hint = "The CSharp code to run.", UIHint = ActivityInputUIHints.CodeEditor, OptionsProvider = typeof(RunCSharp))]
    public string Script { get; set; }

    [ActivityInput(
        Hint = "The possible outcomes that can be set by the script.",
        UIHint = ActivityInputUIHints.MultiText,
        DefaultSyntax = SyntaxNames.Json,
        SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid, CSharpSyntaxName.CSharp },
        ConsiderValuesAsOutcomes = true
    )]
    public ICollection<string> PossibleOutcomes { get; set; } = new List<string> { OutcomeNames.Done };

    [ActivityOutput]
    public object Output { get; set; }

    private readonly ICSharpService _cSharpService;

    public RunCSharp(ICSharpService cSharpService)
    {
        _cSharpService = cSharpService;
    }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        var script = Script;

        if (string.IsNullOrWhiteSpace(script))
            return Done();

        var outcomes = new List<string>();

        var setOutcome = (string value) => outcomes.Add(value);
        var setOutcomes = (IEnumerable<string> values) => outcomes.AddRange(values.Distinct());

        Output = await _cSharpService.EvaluateAsync(script, typeof(object), context, (globalConfigure) =>
        {
            globalConfigure.Context.SetOutcome = setOutcome;
            globalConfigure.Context.SetOutcomes = setOutcomes;
        }, context.CancellationToken);

        // distinct
        outcomes = outcomes.Distinct().ToList();

        if (outcomes.Count == 0)
            outcomes.Add(OutcomeNames.Done);

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
            Syntax = CSharpSyntaxName.CSharp,
        };
    }

    public Task Handle(CSharpTypeDefinitionNotification notification, CancellationToken cancellationToken)
    {
        var source = notification.DefinitionSource;

        source.AppendLine("public static void SetOutcome(string value) => throw new System.NotImplementedException(); ")
            .AppendLine("public static void SetOutcomes(IEnumerable<string> values) => throw new System.NotImplementedException(); ");

        return Task.CompletedTask;
    }
}
