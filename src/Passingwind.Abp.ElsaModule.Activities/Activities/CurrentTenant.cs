using System;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Activities
{
    [Activity(
        Category = "Abp",
        DisplayName = "Current Tenant",
        Description = "",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class CurrentTenant : Activity
    {
        [ActivityOutput]
        public CurrentTenantOutputModel Output { get; set; }

        private readonly ICurrentTenant _currentTenant;

        public CurrentTenant(ICurrentTenant currentTenant)
        {
            _currentTenant = currentTenant;
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            return Execute();
        }
         
        private IActivityExecutionResult Execute()
        {
            Output = new CurrentTenantOutputModel
            {
                Id = _currentTenant.Id,
                Name = _currentTenant.Name,
                IsAvailable = _currentTenant.IsAvailable,
            };

            return Done();
        }
    }

    public class CurrentTenantOutputModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
    }
}
