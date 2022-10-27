using System.Collections.Generic;
using Elsa.Scripting.JavaScript.Options;
using Elsa.Services.Models;
using MediatR;

namespace Passingwind.Abp.ElsaModule.Services
{
    public class CSharpEvaluationContextConfigureEventNotification : INotification
    {
        public CSharpEvaluationContextConfigureEventNotification(string programText, ScriptOptions scriptOptions, ActivityExecutionContext activityExecutionContext)
        {
            ProgramText = programText; 
            ScriptOptions = scriptOptions;
            ActivityExecutionContext = activityExecutionContext;

            GlobalValues = new Dictionary<string, object>();
        }

        public string ProgramText { get; }
         
        public ScriptOptions ScriptOptions { get; }

        public ActivityExecutionContext ActivityExecutionContext { get; }

        public Dictionary<string, object> GlobalValues { get; }

        public void SetValue(string name, object value)
        {
            GlobalValues[name] = value;
        }
    }
}
