using System;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using Volo.Abp.Users;

namespace Passingwind.Abp.ElsaModule.Activities
{
    [Action(
        Category = "Abp",
        DisplayName = "Current User",
        Description = "Get current user info",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class CurrentUser : Activity
    {
        [ActivityOutput(Hint = "The current user info.")]
        public CurrentUserModel Output { get; set; }

        public readonly ICurrentUser _currentUser;

        public CurrentUser(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var model = new CurrentUserModel()
            {
                IsAuthenticated = _currentUser.IsAuthenticated,
                Id = _currentUser.Id,
                Email = _currentUser.Email,
                EmailVerified = _currentUser.EmailVerified,
                Name = _currentUser.Name,
                PhoneNumber = _currentUser.PhoneNumber,
                PhoneNumberVerified = _currentUser.PhoneNumberVerified,
                Roles = _currentUser.Roles,
                SurName = _currentUser.SurName,
                TenantId = _currentUser.TenantId,
                UserName = _currentUser.UserName,
            };

            Output = model;

            return Done();
        }
    }

    public class CurrentUserModel
    {
        public bool IsAuthenticated { get; set; }

        public Guid? Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberVerified { get; set; }

        public string Email { get; set; }

        public bool EmailVerified { get; set; }

        public Guid? TenantId { get; set; }

        public string[] Roles { get; set; }
    }
}
