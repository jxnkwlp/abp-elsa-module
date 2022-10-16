using System;
using System.Collections.Generic;
using System.Linq;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace Passingwind.Abp.ElsaModule.Activities
{
    public class UserTaskV2 : Activity
    {
        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            Hint = "Provide a list of available actions",
            DefaultSyntax = SyntaxNames.Json,
            SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid },
            ConsiderValuesAsOutcomes = true
        )]
        public ICollection<string> Actions { get; set; } = new List<string>();

        public ICollection<string> Users { get; set; }

        public ICollection<string> Roles { get; set; }

        [ActivityOutput]
        public string Output { get; set; }

        protected override bool OnCanExecute(ActivityExecutionContext context)
        {
            var userAction = GetUserAction(context);

            return Actions.Contains(userAction, StringComparer.OrdinalIgnoreCase);
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context) => Suspend();

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            var userAction = GetUserAction(context);
            Output = userAction;
            return Outcome(userAction);
        }

        private static string GetUserAction(ActivityExecutionContext context) => (string)context.Input!;

    }
}
