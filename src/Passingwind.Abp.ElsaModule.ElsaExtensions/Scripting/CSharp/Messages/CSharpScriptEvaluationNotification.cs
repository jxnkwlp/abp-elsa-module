using Elsa.Services.Models;
using MediatR;
using Passingwind.Abp.ElsaModule.CSharp;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

public class CSharpScriptEvaluationNotification : INotification
{
    public CSharpScriptEvaluationNotification(string programText, ActivityExecutionContext activityExecutionContext, CSharpScriptEvaluationGlobal evaluationGlobal)
    {
        ProgramText = programText;
        ActivityExecutionContext = activityExecutionContext;
        EvaluationGlobal = evaluationGlobal;
    }

    public string ProgramText { get; }
    public ActivityExecutionContext ActivityExecutionContext { get; }
    public CSharpScriptEvaluationGlobal EvaluationGlobal { get; }
}
