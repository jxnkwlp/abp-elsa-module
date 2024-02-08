using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
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
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Services;
using Volo.Abp.Emailing;

namespace Passingwind.Abp.ElsaModule.Activities.Emailing;

[Activity(
    Category = "Email",
    DisplayName = "Send Email V2",
    Description = "Send an email message by system 'EmailSender'.",
    Outcomes = new[] { OutcomeNames.Done, "Error" }
)]
public class SendEmailV2 : Activity, IActivityPropertyOptionsProvider, IRuntimeSelectListProvider
{
    [ActivityInput(
        Hint = "The sender's email address. ",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string From { get; set; }

    [ActivityInput(
       Label = "To Users",
       Hint = "The users that want receive email.",
       UIHint = ActivityInputUIHints.MultiText,
       SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid },
       Category = "Security",
       OptionsProvider = typeof(SendEmailV2)
    )]
    public ICollection<string> ToUsers
    {
        get => GetState<ICollection<string>>();
        set => SetState(value);
    }

    [ActivityInput(
        Label = "To Roles",
        Hint = "The role users that want receive email.",
        UIHint = ActivityInputUIHints.MultiText,
        SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        Category = "Security",
       OptionsProvider = typeof(SendEmailV2)
    )]
    public ICollection<string> ToRoles
    {
        get => GetState<ICollection<string>>();
        set => SetState(value);
    }

    [ActivityInput(Label = "Other Emails", Hint = "The recipients email addresses.", UIHint = ActivityInputUIHints.MultiText, DefaultSyntax = SyntaxNames.Json, SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript })]
    public ICollection<string> OtherEmails { get; set; } = new List<string>();

    //[ActivityInput(
    //    Hint = "The cc recipients email addresses.",
    //    UIHint = ActivityInputUIHints.MultiText,
    //    DefaultSyntax = SyntaxNames.Json,
    //    SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript },
    //    Category = "More")]
    //public ICollection<string> Cc { get; set; } = new List<string>();

    //[ActivityInput(
    //    Hint = "The Bcc recipients email addresses.",
    //    UIHint = ActivityInputUIHints.MultiText,
    //    DefaultSyntax = SyntaxNames.Json,
    //    SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript },
    //    Category = "More")]
    //public ICollection<string> Bcc { get; set; } = new List<string>();

    [ActivityInput(Hint = "The subject of the email message.", SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid }, IsDesignerCritical = true)]
    public string Subject { get; set; }

    [ActivityInput(Hint = "The body of the email message.", UIHint = ActivityInputUIHints.MultiLine, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string Body { get; set; }

    [ActivityInput(
        Hint = "Queue the email to background."
        )]
    public bool Queued { get; set; }

    private readonly ILogger<SendEmailV2> _logger;
    private readonly IEmailSender _emailSender;
    private readonly IUserLookupService _userLookupService;
    private readonly IRoleLookupService _roleLookupService;

    public SendEmailV2(ILogger<SendEmailV2> logger, IEmailSender emailSender, IUserLookupService userLookupService, IRoleLookupService roleLookupService)
    {
        _logger = logger;
        _emailSender = emailSender;
        _userLookupService = userLookupService;
        _roleLookupService = roleLookupService;
    }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        var a = this;
        var message = new MailMessage()
        {
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            Subject = Subject,
            Body = Body ?? string.Empty,
        };

        if (!string.IsNullOrEmpty(From))
            message.From = new MailAddress(From);

        if (OtherEmails?.Any() == true)
        {
            foreach (var item in OtherEmails)
            {
                message.To.Add(item);
            }
        }

        if (ToUsers?.Any() == true)
        {
            foreach (var item in ToUsers)
            {
                var user = await _userLookupService.FindByUserNameAsync(item);
                if (user != null)
                    message.To.Add(user.Email);
            }
        }

        if (ToRoles?.Any() == true)
        {
            foreach (var item in ToRoles)
            {
                var users = await _userLookupService.GetListByRoleNameAsync(item);
                foreach (var user in users)
                {
                    message.To.Add(user.Email);
                }
            }
        }

        //if (Cc?.Any() == true)
        //    foreach (var item in Cc)
        //    {
        //        message.CC.Add(item);
        //    }

        //if (Bcc?.Any() == true)
        //    foreach (var item in Bcc)
        //    {
        //        message.Bcc.Add(item);
        //    }

        try
        {
            if (Queued)
            {
                if (message.From != null)
                    await _emailSender.QueueAsync(message.From.ToString(), message.To.ToString(), message.Subject, message.Body);
                else
                    await _emailSender.QueueAsync(message.To.ToString(), message.Subject, message.Body);
            }
            else
            {
                await _emailSender.SendAsync(message);
            }

            return Done();
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Send email failed.");
            context.JournalData.Add("Error", ex.Message);
            return Outcome("Error");
        }
    }

    public object GetOptions(PropertyInfo property)
    {
        if (property.Name == nameof(ToUsers))
        {
            return new RuntimeSelectListProviderSettings(GetType(), new SendEmailV2GetRuntimeSelectListContext()
            {
                Name = "Users"
            });
        }
        else if (property.Name == nameof(ToRoles))
        {
            return new RuntimeSelectListProviderSettings(GetType(), new SendEmailV2GetRuntimeSelectListContext()
            {
                Name = "Roles"
            });
        }

        return default;
    }

    public async ValueTask<SelectList> GetSelectListAsync(object context = null, CancellationToken cancellationToken = default)
    {
        if (context != null && context is SendEmailV2GetRuntimeSelectListContext context2)
        {
            if (context2.Name == "Users")
            {
                var list = await _userLookupService.GetListAsync(cancellationToken: cancellationToken);
                return new SelectList(list?.Select(x => new SelectListItem(x.DisplayName, x.UserName)).ToArray());
            }
            else if (context2.Name == "Roles")
            {
                var list = await _roleLookupService.GetListAsync(cancellationToken: cancellationToken);
                return new SelectList(list?.Select(x => new SelectListItem(x.Name, x.Name)).ToArray());
            }
        }

        return default;
    }
}

public class SendEmailV2GetRuntimeSelectListContext
{
    public string Name { get; set; }
}
