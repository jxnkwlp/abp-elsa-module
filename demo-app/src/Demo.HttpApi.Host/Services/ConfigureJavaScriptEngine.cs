using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.JavaScript.Events;
using Elsa.Scripting.JavaScript.Messages;
using MediatR;

namespace Demo.Services
{
    public class ConfigureJavaScriptEngine : INotificationHandler<EvaluatingJavaScriptExpression>
    {
        public Task Handle(EvaluatingJavaScriptExpression notification, CancellationToken cancellationToken)
        {
            var engine = notification.Engine;

            engine.SetValue("formatDateTime", (Func<DateTime, string, string>)((source, temp) => source.ToString(temp)));

            return Task.CompletedTask;
        }
    }

    public class JavaScriptTypeDefinitions : INotificationHandler<RenderingTypeScriptDefinitions>
    {
        public Task Handle(RenderingTypeScriptDefinitions notification, CancellationToken cancellationToken)
        {
            var output = notification.Output;

            output.AppendLine("declare function formatDateTime(dateTime: DateTime, format: string): string");

            return Task.CompletedTask;
        }
    }
}
