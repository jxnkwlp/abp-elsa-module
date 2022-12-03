using Microsoft.AspNetCore.Authentication;

namespace Demo.ApiKeys;

public class ApiKeyOptions : AuthenticationSchemeOptions
{
    public string KeyName { get; set; }
}
