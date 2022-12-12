//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Elsa.Activities.Http;
//using Elsa.Events;
//using Elsa.Expressions;
//using MediatR;

//namespace Passingwind.Abp.ElsaModule.Activities.Http
//{
//    public class SendHttpRequestConfigureProperty : INotificationHandler<DescribingActivityType>
//    {
//        public Task Handle(DescribingActivityType notification, CancellationToken cancellationToken)
//        {
//            var activityType = notification.ActivityType;

//            if (activityType.Type != typeof(SendHttpRequest))
//                return Task.CompletedTask;

//            var inputProperties = notification.ActivityDescriptor.InputProperties.ToList();

//            inputProperties.Add(new Elsa.Metadata.ActivityInputDescriptor("Timeout", typeof(int?), "Timeout", "Timeout", hint: "Request timeout, in seconds", supportedSyntaxes: new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }));

//            notification.ActivityDescriptor.InputProperties = inputProperties.ToArray();

//            return Task.CompletedTask;
//        }
//    }
//}
