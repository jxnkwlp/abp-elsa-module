using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Temporal;
using Elsa.Events;
using MediatR;

namespace Passingwind.WorkflowApp.Web.Services;

public class HangfireCronPropertyConfigure : INotificationHandler<DescribingActivityType>
{
    public Task Handle(DescribingActivityType notification, CancellationToken cancellationToken)
    {
        var activityType = notification.ActivityType;

        if (activityType.Type != typeof(Cron))
            return Task.CompletedTask;

        var cronExpressionProperty = notification.ActivityDescriptor.InputProperties.First(x => x.Name == nameof(Cron.CronExpression));
        cronExpressionProperty.DefaultValue = "* * * * *";
        cronExpressionProperty.Hint = "Specify a CRON expression. Go to https://ncrontab.swimburger.net/ to generate valid cron expressions.";

        return Task.CompletedTask;
    }
}