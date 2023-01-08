using System;
using System.ComponentModel.DataAnnotations;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Configuration;

namespace Passingwind.Abp.ElsaModule.Activities.Configuration;

[Activity(
    Category = "Configuration",
    DisplayName = "Read Configuration",
    Outcomes = new[] { OutcomeNames.Done }
)]
public class ReadConfiguration : Activity
{
    [ActivityInput(
        Label = "Key",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        IsDesignerCritical = true)]
    [Required]
    public string Key { get; set; }

    [ActivityOutput]
    public string Value { get; set; }

    private readonly IConfiguration _configuration;

    public ReadConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
    {
        return Execute();
    }

    private IActivityExecutionResult Execute()
    {
        if (string.IsNullOrEmpty(Key))
            throw new ArgumentNullException(nameof(Key));

        Value = _configuration[Key];

        return Done();
    }

}
