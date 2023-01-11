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
    DisplayName = "WeChat Work Bot",
    Outcomes = new[] { OutcomeNames.Done }
)]
public class WeChatWorkGroupBot : Activity, IActivityPropertyOptionsProvider
{
    [ActivityInput(
        Label = "Key",
        Hint = "see https://work.weixin.qq.com/api/doc/90000/90136/91770",
        IsDesignerCritical = true,
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    [Required]
    public string Key { get; set; }

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
        Label = "Content",
        Hint = "",
        UIHint = ActivityInputUIHints.MultiLine,
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string Content { get; set; }

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

    private const string _urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key={0}";
    private readonly ILogger<WeChatWorkGroupBot> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public WeChatWorkGroupBot(ILogger<WeChatWorkGroupBot> logger, IHttpClientFactory httpClientFactory)
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
                mentioned_list = MentionedList,
                mentioned_mobile_list = MentionedMobileList,
            },
            markdown = new
            {
                content = Content,
                mentioned_list = MentionedList,
                mentioned_mobile_list = MentionedMobileList,
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(string.Format(_urlFormat, Key), content, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            response.EnsureSuccessStatusCode();

            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<WebHookResponse>();
                context.JournalData.Add("Result", responseContent);

                if (responseContent.ErrCode != 0)
                {
                    return Fault($"Send wechat work bot message failed: {responseContent.ErrMsg}");
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
                new SelectListItem("Markdown", "markdown")
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
