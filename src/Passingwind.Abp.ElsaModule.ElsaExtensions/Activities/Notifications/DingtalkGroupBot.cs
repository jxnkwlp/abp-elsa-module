using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
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

namespace Passingwind.Abp.ElsaModule.Activities.Notifications;

[Action(
    Category = "Notifications",
    DisplayName = "Dingtalk Bot",
    Outcomes = new[] { OutcomeNames.Done }
)]
public class DingtalkGroupBot : Activity, IActivityPropertyOptionsProvider
{
    [ActivityInput(
        Label = "Access Token",
        Hint = "see https://open.dingtalk.com/document/robots/custom-robot-access",
        IsDesignerCritical = true,
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    [Required]
    public string AccessToken { get; set; }

    [ActivityInput(
        Label = "Message Type",
        Hint = "",
        IsDesignerCritical = true,
        UIHint = ActivityInputUIHints.Dropdown,
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        OptionsProvider = typeof(WeChatWorkGroupBot))]
    [Required]
    public string MessageType { get; set; }

    [ActivityInput(
        Label = "Title",
        Hint = "",
        UIHint = ActivityInputUIHints.MultiLine,
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string Title { get; set; }

    [ActivityInput(
        Label = "Content",
        Hint = "",
        UIHint = ActivityInputUIHints.MultiLine,
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string Content { get; set; }

    [ActivityInput(
        Label = "Picture Url",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string PictureUrl { get; set; }

    [ActivityInput(
        Label = "Message Url",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string MessageUrl { get; set; }

    [ActivityInput(
        Label = "Mentioned List",
        Hint = "",
        UIHint = ActivityInputUIHints.MultiText,
        SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public ICollection<string> MentionedList { get; set; }

    [ActivityInput(
        Label = "Mentioned Mobile List",
        Hint = "",
        UIHint = ActivityInputUIHints.MultiText,
        SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public ICollection<string> MentionedMobileList { get; set; }

    [ActivityInput(
        Label = "Mentione All",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public bool MentionedAll { get; set; }

    private const string _urlFormat = "https://oapi.dingtalk.com/robot/send?access_token={0}";
    private readonly ILogger<WeChatWorkGroupBot> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public DingtalkGroupBot(ILogger<WeChatWorkGroupBot> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient(nameof(WeChatWorkGroupBot));
    }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        var content = new
        {
            msgtype = MessageType,
            text = new
            {
                content = Content,
            },
            markdown = new
            {
                title = Title,
                text = Content,
            },
            link = new
            {
                title = Title,
                text = Content,
                messageUrl = MessageUrl,
                picUrl = PictureUrl,
            },
            at = new
            {
                atMobiles = MentionedMobileList,
                atUserIds = MentionedList,
                isAtAll = MentionedAll,
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(string.Format(_urlFormat, AccessToken), content, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            response.EnsureSuccessStatusCode();

            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<WebHookResponse>();
                context.JournalData.Add("Result", responseContent);

                if (responseContent.ErrCode != 0)
                {
                    return Fault($"Send dingtalk bot message failed: {responseContent.ErrMsg}");
                }
            }

            return Done();
        }
        catch (Exception ex)
        {
            return Fault(ex);
        }
    }

    public object GetOptions(PropertyInfo property)
    {
        if (property.Name == nameof(MessageType))
        {
            return new List<SelectListItem>() {
                new SelectListItem("Text", "text"),
                new SelectListItem("Markdown", "markdown"),
                new SelectListItem("Link", "link"),
            };
        }

        return null;
    }

    public class WebHookResponse
    {
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
    }
}

