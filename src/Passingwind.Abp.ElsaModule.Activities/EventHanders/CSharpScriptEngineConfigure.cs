using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using Passingwind.Abp.ElsaModule.Services;

namespace Passingwind.Abp.ElsaModule.EventHanders
{
    public class CSharpScriptEngineConfigure : INotificationHandler<CSharpEvaluationContextConfigureEventNotification>
    {
        public Task Handle(CSharpEvaluationContextConfigureEventNotification notification, CancellationToken cancellationToken)
        {
            var activityExecutionContext = notification.ActivityExecutionContext;
            var workflowExecutionContext = activityExecutionContext.WorkflowExecutionContext;
            var workflowInstance = workflowExecutionContext.WorkflowInstance;

            dynamic context = notification.EvaluationGlobal.Context;

            // Global functions.
            context.SetVariable = (Action<string, object>)((name, value) => activityExecutionContext.SetVariable(name, ProcessVariable(value)));
            context.GetVariable = (Func<string, object>)(name => ProcessVariable(activityExecutionContext.GetVariable(name)));


            return Task.CompletedTask;
        }

        /// <summary>
        /// If the variable is a <see cref="JObject"/> or a <see cref="JArray"/>, convert it into an <see cref="ExpandoObject"/> or a list thereof, respectively.
        /// Jint will then be able to deal with them as if they were native JavaScript objects and do things like JSON.stringify.
        /// If the variable is an <see cref="ExpandoObject"/> or a list thereof, then it is converted back into a <see cref="JObject"/> or a <see cref="JArray"/>. 
        /// </summary>
        private object ProcessVariable(object? value) =>
            value switch
            {
                JArray jArray => jArray.Select(ProcessVariable).ToList(),
                JObject jObject => jObject.ToObject<ExpandoObject>(),
                ExpandoObject expandoObject => JObject.FromObject(expandoObject),
                ICollection<ExpandoObject> expandoObjects => new JArray(expandoObjects.Select(JObject.FromObject)),
                JToken jToken => jToken.Type switch
                {
                    JTokenType.Boolean => jToken.Value<bool>(),
                    JTokenType.Bytes => jToken.Value<byte[]>(),
                    JTokenType.Date => jToken.Value<DateTimeOffset>(),
                    JTokenType.Float => jToken.Value<float>(),
                    JTokenType.Guid => jToken.Value<Guid>(),
                    JTokenType.Integer => jToken.Value<int>(),
                    JTokenType.Object => jToken.ToObject<ExpandoObject>(),
                    _ => jToken.ToString()
                },
                _ => value
            };
    }
}
