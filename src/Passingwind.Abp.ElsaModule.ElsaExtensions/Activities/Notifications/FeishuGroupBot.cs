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
    DisplayName = "Feishu Bot",
    Outcomes = new[] { OutcomeNames.Done }
)]
public class FeishuGroupBot : Activity, IActivityPropertyOptionsProvider
{
    [ActivityInput(
        Label = "Access Token",
        Hint = "",
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
        Label = "Content",
        Hint = "",
        UIHint = ActivityInputUIHints.MultiLine,
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string Content { get; set; }


    private const string _urlFormat = "https://open.feishu.cn/open-apis/bot/v2/hook/{0}";
    private readonly ILogger<WeChatWorkGroupBot> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public FeishuGroupBot(ILogger<WeChatWorkGroupBot> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient(nameof(WeChatWorkGroupBot));
    }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        var content = new
        {
            msg_type = MessageType,
            content = new
            {
                text = Content,
            },
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(string.Format(_urlFormat, AccessToken), content, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            response.EnsureSuccessStatusCode();

            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<WebHookResponse>();
                context.JournalData.Add("Result", responseContent);

                if (responseContent.Code != 0)
                {
                    return Fault($"Send feishu group bot message failed: {responseContent.Msg}");
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
            };
        }

        return null;
    }

    public class WebHookResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
}

