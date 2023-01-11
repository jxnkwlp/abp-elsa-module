using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
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
using IdentityModel.Client;
using Microsoft.Extensions.Logging;

namespace Passingwind.Abp.ElsaModule.Activities.OAuth2;

[Action(
    Category = "HTTP",
    DisplayName = "OAuth2 Client",
    Outcomes = new[] { OutcomeNames.Done }
)]
public class OAuth2Client : Activity, IActivityPropertyOptionsProvider
{
    [ActivityInput(
        Label = "Access Token URL",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string AccessTokenURL { get; set; }

    [ActivityInput(
        Label = "Grant Type",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        OptionsProvider = typeof(OAuth2Client),
        UIHint = ActivityInputUIHints.Dropdown)]
    [Required]
    public string GrantType { get; set; }

    [ActivityInput(
        Label = "Client Id",
        Hint = "The client id",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        IsDesignerCritical = true)]
    [Required]
    public string ClientId { get; set; }

    [ActivityInput(
        Label = "Client Secret",
        Hint = "The client secret if required",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string ClientSecret { get; set; }

    [ActivityInput(
        Label = "Scope",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string Scope { get; set; }

    [ActivityInput(
        Label = "UserName",
        Hint = "The username for password grant type",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        DependsOnEvents = new[] { "GrantType" },
        DependsOnValues = new[] { "password" })]
    public string UserName { get; set; }

    [ActivityInput(
        Label = "Password",
        Hint = "The password for password grant type",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string Password { get; set; }

    [ActivityInput(
        Label = "Request Timeout",
        Hint = "The request timeout(seconds)",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public int? RequestTimeout { get; set; }

    [ActivityOutput()]
    public OAuth2ClientOutputModel Output { get; set; }

    private readonly HttpClient _httpClient;
    private readonly ILogger<OAuth2Client> _logger;
    private readonly IHttpClientFactory _httpClientFactory;


    public OAuth2Client(ILogger<OAuth2Client> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(nameof(OAuth2Client));
    }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        TokenResponse tokenResponse = null;

        if (RequestTimeout.HasValue)
            _httpClient.Timeout = TimeSpan.FromSeconds(RequestTimeout.Value);

        if (GrantType == "password")
        {
            tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = AccessTokenURL,
                GrantType = GrantType,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Scope = Scope,

                UserName = UserName,
                Password = Password,
            });
        }
        else if (GrantType == "client_credentials")
        {
            tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = AccessTokenURL,
                GrantType = GrantType,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Scope = Scope,
            });
        }
        else
        {
            return Fault($"OAuth2 token request unsupport grant type '{GrantType}' ");
        }

        if (tokenResponse?.IsError == true)
        {
            _logger.LogError(message: $"OAuth2 token request failed.\n error: {tokenResponse.Error}, description: {tokenResponse.ErrorDescription}");

            context.JournalData.Add("Error", tokenResponse.Error);
            context.JournalData.Add("ErrorDescription", tokenResponse.ErrorDescription);
            context.JournalData.Add("ErrorType", tokenResponse.ErrorType);

            return Fault($"OAuth2 token request failed with error '{tokenResponse.Error}' ");
        }

        Output = new OAuth2ClientOutputModel()
        {
            AccessToken = tokenResponse.AccessToken,
            IdentityToken = tokenResponse.IdentityToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn,
            TokenType = tokenResponse.TokenType,
            Scope = tokenResponse.Scope,
        };

        return Done(Output);
    }

    public object GetOptions(PropertyInfo property)
    {
        if (property.Name == nameof(GrantType))
        {
            return new List<SelectListItem>() {
                new SelectListItem("Client Credentials", "client_credentials"),
                new SelectListItem("Password Credentials", "password")
            };
        }

        return null;
    }

}

public class OAuth2ClientOutputModel
{
    public string AccessToken { get; set; }
    public string IdentityToken { get; set; }
    public string RefreshToken { get; set; }
    public string TokenType { get; set; }
    public string Scope { get; set; }
    public int ExpiresIn { get; set; }
}
