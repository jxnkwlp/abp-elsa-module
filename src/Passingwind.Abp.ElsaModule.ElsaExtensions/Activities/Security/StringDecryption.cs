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
    Category = "Abp",
    DisplayName = "String Decryption",
    Description = "",
    Outcomes = new[] { OutcomeNames.Done, "Error" }
)]
public class StringDecryption : Activity
{
    [ActivityInput(
        Label = "Cipher Text",
        Hint = "The cipher text ",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        IsDesignerCritical = true)]
    [Required]
    public string CipherText { get; set; }

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

    public StringDecryption(ILogger<StringDecryption> logger, IStringEncryptionService stringEncryptionService)
    {
        _logger = logger;
        _stringEncryptionService = stringEncryptionService;
    }

    protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
    {
        try
        {
            Output = _stringEncryptionService.Decrypt(CipherText, PassPhrase, Salt);

            return Done();
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Decrypting text failed.");
            context.JournalData.Add("Error", ex.Message);
            return Outcome("Error");
        }
    }

}
