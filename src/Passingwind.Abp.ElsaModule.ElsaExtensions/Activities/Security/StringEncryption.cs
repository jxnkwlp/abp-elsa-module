using System.ComponentModel.DataAnnotations;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using Volo.Abp.Security.Encryption;

namespace Passingwind.Abp.ElsaModule.Activities.Security;

[Activity(
    Category = "Security",
    DisplayName = "String Encryption",
    Description = "",
    Outcomes = new[] { OutcomeNames.Done, "Error" }
)]
public class StringEncryption : Activity
{
    [ActivityInput(
            Label = "Plain Text",
            Hint = "The plain text ",
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
            IsDesignerCritical = true)]
    [Required]
    public string PlainText { get; set; }

    [ActivityInput(
        Label = "Pass Phrase",
        Hint = "The pass phrase ",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string PassPhrase { get; set; }

    [ActivityInput(
        Label = "Salt",
        Hint = "The salt",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript })]
    public byte[] Salt { get; set; }

    [ActivityOutput]
    public string Output { get; set; }

    private readonly ILogger<StringDecryption> _logger;
    private readonly IStringEncryptionService _stringEncryptionService;

    public StringEncryption(ILogger<StringDecryption> logger, IStringEncryptionService stringEncryptionService)
    {
        _logger = logger;
        _stringEncryptionService = stringEncryptionService;
    }

    protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
    {
        try
        {
            Output = _stringEncryptionService.Encrypt(PlainText, PassPhrase, Salt);

            return Done();
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Encrypting text failed.");
            context.JournalData.Add("Error", ex.Message);
            return Outcome("Error");
        }
    }

}
