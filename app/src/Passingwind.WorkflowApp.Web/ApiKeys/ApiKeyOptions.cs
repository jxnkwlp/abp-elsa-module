using Microsoft.AspNetCore.Authentication;

namespace Passingwind.WorkflowApp.Web.ApiKeys;

public class ApiKeyOptions : AuthenticationSchemeOptions
{
    public string KeyName { get; set; }
}
