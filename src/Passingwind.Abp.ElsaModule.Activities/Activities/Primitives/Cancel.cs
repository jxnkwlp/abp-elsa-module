using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace Passingwind.Abp.ElsaModule.Activities.Primitives
{
    [Activity(Category = "Primitives", Description = "Put the workflow in a cancel state.", Outcomes = new string[0])]
    public class Cancel : Activity
    {
        /// <summary>
        /// Optional.
        /// </summary>
        [ActivityInput(Hint = "Optional. The message to store as the reason for the cancel.", SupportedSyntaxes = new[] { SyntaxNames.Liquid, SyntaxNames.JavaScript })]
        public string Message { get; set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var message = Message ?? "Custom canceled";
            context.JournalData.Add("Error", message);

            context.WorkflowExecutionContext.Cancel(new System.Exception("Custom canceled."), message, context.ActivityBlueprint.Id, context.Input, context.Resuming);

            return Noop();
        }
    }
}
