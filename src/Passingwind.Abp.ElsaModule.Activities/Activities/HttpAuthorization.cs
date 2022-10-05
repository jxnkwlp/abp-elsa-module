using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Passingwind.Abp.ElsaModule.Services;
using Volo.Abp.Users;

namespace Passingwind.Abp.ElsaModule.Activities
{
    [Activity(
        Category = "HTTP",
        DisplayName = "HTTP Authorization",
        Description = "Check incoming request authorization",
        Outcomes = new[] { OutcomeNames.Done, FailedOutcome }
    )]
    public class HttpAuthorization : Activity, IActivityPropertyOptionsProvider, IRuntimeSelectListProvider
    {
        private const string FailedOutcome = "Failed";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<HttpAuthorization> _localizer;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrentUser _currentUser;
        private readonly IUserLookupService _userLookupService;
        private readonly IRoleLookupService _roleLookupService;

        public HttpAuthorization(IHttpContextAccessor httpContextAccessor, IStringLocalizer<HttpAuthorization> localizer, IAuthorizationService authorizationService, ICurrentUser currentUser, IUserLookupService userLookupService, IRoleLookupService roleLookupService)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
            _authorizationService = authorizationService;
            _currentUser = currentUser;
            _userLookupService = userLookupService;
            _roleLookupService = roleLookupService;
        }

        [ActivityInput(
            Label = "Fault",
            Hint = "As fault if not authorization or forbidden",
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public bool IsFault { get; set; } = default!;

        [ActivityInput(
            Hint = "Provide a policy to evaluate. If the policy fails, the request is forbidden.",
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
            Category = "Security"
        )]
        public string Policy { get; set; }

        [ActivityInput(
           Label = "Users",
           Hint = "Provide users as a condition for authorization check",
           UIHint = ActivityInputUIHints.MultiText,
           SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid },
           Category = "Security",
           OptionsProvider = typeof(HttpAuthorization)
        )]
        public ICollection<string> RequiredUsers { get; set; }

        [ActivityInput(
            Label = "Roles",
            Hint = "Provide roles as a condition for authorization check",
            UIHint = ActivityInputUIHints.MultiText,
            SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid },
            Category = "Security",
           OptionsProvider = typeof(HttpAuthorization)
        )]
        public ICollection<string> RequiredRoles { get; set; }

        public object GetOptions(PropertyInfo property)
        {
            if (property.Name == nameof(RequiredUsers))
                return new RuntimeSelectListProviderSettings(GetType(), new HttpAuthorizationGetRuntimeSelectListContext()
                {
                    Name = "Users"
                });

            else if (property.Name == nameof(RequiredRoles))
                return new RuntimeSelectListProviderSettings(GetType(), new HttpAuthorizationGetRuntimeSelectListContext()
                {
                    Name = "Roles"
                });

            return default;
        }

        public async ValueTask<SelectList> GetSelectListAsync(object context = null, CancellationToken cancellationToken = default)
        {
            if (context != null && context is HttpAuthorizationGetRuntimeSelectListContext context2)
            {
                if (context2.Name == "Users")
                {
                    var list = await _userLookupService.SearchAsync();
                    return new SelectList(list?.Select(x => new SelectListItem(x.DisplayName, x.UserName)).ToArray());
                }
                else if (context2.Name == "Roles")
                {
                    var list = await _roleLookupService.SearchAsync();
                    return new SelectList(list?.Select(x => new SelectListItem(x.Name, x.Name)).ToArray());
                }
            }

            return default;
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.User?.Identity?.IsAuthenticated != true)
                return HandleFaultResult(401, _localizer["Request is unauthenticated"]);

            // Policy
            if (!string.IsNullOrEmpty(Policy))
            {
                var result = await _authorizationService.AuthorizeAsync(httpContext.User, Policy);

                if (!result.Succeeded)
                {
                    return HandleFaultResult(403, _localizer["Request is forbidden"]);
                }
            }

            // users
            if (RequiredUsers?.Any() == true)
            {
                if (!RequiredUsers.Contains(_currentUser.UserName) && !RequiredUsers.Contains(_currentUser.Email))
                {
                    return HandleFaultResult(401, _localizer["Request is forbidden"]);
                }
            }

            // roles
            if (RequiredRoles?.Any() == true)
            {
                var userRoles = _currentUser.Roles;
                if (!RequiredRoles.Intersect(userRoles).Any())
                {
                    return HandleFaultResult(403, _localizer["Request is forbidden"]);
                }
            }

            return Done();
        }

        private IActivityExecutionResult HandleFaultResult(int statusCode, string faultMessage)
        {
            if (IsFault)
                return Fault(faultMessage);
            else
                return Combine(new HttpAuthorizationFaultExecutionResult(_httpContextAccessor, statusCode), Outcome(FailedOutcome));
        }

    }

    public class HttpAuthorizationFaultExecutionResult : ActivityExecutionResult
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _statusCode;

        public HttpAuthorizationFaultExecutionResult(IHttpContextAccessor httpContextAccessor, int statusCode)
        {
            _httpContextAccessor = httpContextAccessor;
            _statusCode = statusCode;
        }

        protected override void Execute(ActivityExecutionContext activityExecutionContext)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var response = httpContext.Response;

            response.StatusCode = _statusCode;
        }
    }

    public class HttpAuthorizationGetRuntimeSelectListContext
    {
        public string Name { get; set; }
    }
}
