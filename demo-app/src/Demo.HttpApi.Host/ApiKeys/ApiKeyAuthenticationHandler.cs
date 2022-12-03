using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Uow;

namespace Demo.ApiKeys;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyOptions>
{
    private readonly ApiKeyDomainService _domainService;
    private readonly IIdentityUserRepository _userRepository;
    private readonly AbpSignInManager _signInManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ApiKeyDomainService domainService, AbpSignInManager signInManager, IUnitOfWorkManager unitOfWorkManager, IIdentityUserRepository userRepository) : base(options, logger, encoder, clock)
    {
        _domainService = domainService;
        _signInManager = signInManager;
        _unitOfWorkManager = unitOfWorkManager;
        _userRepository = userRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.GetEndpoint()?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
            return AuthenticateResult.NoResult();

        var apiKeyValue = GetApiKeyValue();

        if (string.IsNullOrEmpty(apiKeyValue))
        {
            Logger.LogDebug("Api key not found in the request");
            return AuthenticateResult.NoResult();
        }

        Logger.LogDebug("Api key found in the request. Api key: {apiKeyValue}", apiKeyValue);

        var claimsPrincipal = await CreateClaimsPrincipalAsync(apiKeyValue);
        if (claimsPrincipal == null)
        {
            Logger.LogDebug("Invalid API Key");
            return AuthenticateResult.Fail("Invalid API Key");
        }

        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, ApiKeyDefaults.AuthenticationScheme));
    }

    private string GetApiKeyValue()
    {
        string value = Context.Request.Query[Options.KeyName];

        if (string.IsNullOrEmpty(value))
            value = Context.Request.Headers[Options.KeyName];

        return value;
    }

    private async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(string apiKey)
    {
        using (var uow = _unitOfWorkManager.Begin())
        {
            Guid? userId = await _domainService.ValidateAsync(apiKey);

            if (userId == null)
                return null;

            var user = await _userRepository.FindAsync(userId.Value);

            if (user == null)
                return null;

            if (!await _signInManager.CanSignInAsync(user))
                return null;

            return await _signInManager.CreateUserPrincipalAsync(user);
        }
    }
}
