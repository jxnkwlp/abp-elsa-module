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
using Elsa.Scripting.JavaScript.Services;
using Elsa.Services;
using Elsa.Services.Models;
using Jint;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.GlobalCodes;
using Passingwind.Abp.ElsaModule.Scripting.CSharp;
using Passingwind.CSharpScriptEngine;

namespace Passingwind.Abp.ElsaModule.Activities.Scripting;

[Action(
    DisplayName = "Run Code Block",
    Category = "Scripting",
    Description = "Run global code",
    Outcomes = new[] { OutcomeNames.Done, OutcomeNames.True, OutcomeNames.False })]
public class RunCode : Activity, IRuntimeSelectListProvider, IActivityPropertyOptionsProvider
{
    [ActivityInput(
        Label = "Name",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        OptionsProvider = typeof(RunCode),
        IsDesignerCritical = true,
        UIHint = ActivityInputUIHints.Dropdown)]
    public string CodeName { get; set; }

    [ActivityInput(Label = "Version", SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid }, UIHint = ActivityInputUIHints.SingleLine)]
    public int? Version { get; set; }

    [ActivityInput(
        Hint = "The possible outcomes that can be set by the script.",
        UIHint = ActivityInputUIHints.MultiText,
        DefaultSyntax = SyntaxNames.Json,
        SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        ConsiderValuesAsOutcomes = true
    )]
    public ICollection<string> PossibleOutcomes { get; set; } = new List<string> { OutcomeNames.Done };

    [ActivityOutput]
    public object Output { get; set; }

    private readonly ILogger<RunCode> _logger;
    private readonly GlobalCodeManager _globalCodeManager;
    private readonly IGlobalCodeRepository _globalCodeRepository;
    private readonly ICSharpService _cSharpService;
    private readonly IJavaScriptService _javaScriptService;

    public RunCode(GlobalCodeManager globalCodeManager, IGlobalCodeRepository globalCodeRepository, ICSharpService cSharpService, IJavaScriptService javaScriptService, ILogger<RunCode> logger)
    {
        _globalCodeManager = globalCodeManager;
        _globalCodeRepository = globalCodeRepository;
        _cSharpService = cSharpService;
        _javaScriptService = javaScriptService;
        _logger = logger;
    }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        return await ExecuteInternalAsync(context);
    }

    protected override async ValueTask<IActivityExecutionResult> OnResumeAsync(ActivityExecutionContext context)
    {
        return await ExecuteInternalAsync(context);
    }

    protected async ValueTask<IActivityExecutionResult> ExecuteInternalAsync(ActivityExecutionContext context)
    {
        var entity = await _globalCodeRepository.FindByNameAsync(CodeName, cancellationToken: context.CancellationToken);

        if (entity == null)
        {
            throw new ArgumentException($"The code with name '{CodeName}' is not exists.");
        }

        string codeContent = string.Empty;

        if (!Version.HasValue)
        {
            Version = entity.PublishedVersion;
        }

        codeContent = await _globalCodeManager.GetContentByVersionAsync(CodeName, Version.Value, cancellationToken: context.CancellationToken);

        if (string.IsNullOrWhiteSpace(codeContent))
            return Done();

        var returnType = typeof(object);

        if (entity.Type == GlobalCodeType.Condition)
        {
            returnType = typeof(bool);
        }

        var outcomes = new List<string>();

        var setOutcome = (string value) => outcomes.Add(value);
        var setOutcomes = (IEnumerable<string> values) => outcomes.AddRange(values.Distinct());

        if (entity.Language == GlobalCodeLanguage.Javascript)
        {
            void ConfigureEngine(Engine engine)
            {
                engine
                    .SetValue("setOutcome", setOutcome)
                    .SetValue("setOutcomes", setOutcomes);
            }

            Output = await _javaScriptService.EvaluateAsync(codeContent, returnType, context, ConfigureEngine, context.CancellationToken);
        }
        else if (entity.Language == GlobalCodeLanguage.Javascript)
        {
            void configureGlobal(CSharpScriptEvaluationGlobal c)
            {
                c.Context.SetOutcome = setOutcome;
                c.Context.SetOutcomes = setOutcomes;
            }

            Output = await _cSharpService.EvaluateAsync(codeContent, returnType, context, configureGlobal, context.CancellationToken);
        }
        else
        {
            throw new NotSupportedException($"The code language '{entity.Language}' is not support.");
        }

        context.LogOutputProperty(this, nameof(Output), Output);

        if (Output != null)
        {
            context.JournalData.Add("OutputType", Output.GetType());
        }

        if (entity.Type == GlobalCodeType.Condition)
        {
            if (Output != null)
                outcomes.Add(Convert.ToBoolean(Output) ? OutcomeNames.True : OutcomeNames.False);
            else
                outcomes.Add(OutcomeNames.False);
        }

        // distinct
        outcomes = outcomes.Distinct().ToList();

        if (outcomes.Count == 0)
            outcomes.Add(OutcomeNames.Done);

        context.JournalData.Add("Outcomes", outcomes);

        return Outcomes(outcomes);
    }

    public async ValueTask<SelectList> GetSelectListAsync(object context = null, CancellationToken cancellationToken = default)
    {
        if (context is string name && name == nameof(CodeName))
        {
            var list = await _globalCodeRepository.GetListAsync(cancellationToken: cancellationToken);

            return new SelectList(list.OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Name)).ToArray());
        }

        return default;
    }

    public object GetOptions(PropertyInfo property)
    {
        if (property.Name == nameof(CodeName))
        {
            return new RuntimeSelectListProviderSettings(GetType(), nameof(CodeName));
        }

        return default;
    }
}
