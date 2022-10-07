//using System;
//using Elsa;
//using Elsa.ActivityResults;
//using Elsa.Attributes;
//using Elsa.Services;
//using Elsa.Services.Models;
//using Volo.Abp.MultiTenancy;

//namespace Passingwind.Abp.ElsaModule.Activities
//{
//    [Activity(
//        Category = "Abp",
//        DisplayName = "Change Tenant",
//        Description = "",
//        Outcomes = new[] { OutcomeNames.Done }
//    )]
//    public class TenantChanget : Activity
//    {
//        public Guid Id { get; set; }
//        public string Name { get; set; }

//        private readonly ICurrentTenant _currentTenant;

//        public TenantChanget(ICurrentTenant currentTenant)
//        {
//            _currentTenant = currentTenant;
//        }

//        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
//        {
//            _currentTenant.Change(Id, Name);

//            return base.OnExecute(context);
//        }
//    }
//}
