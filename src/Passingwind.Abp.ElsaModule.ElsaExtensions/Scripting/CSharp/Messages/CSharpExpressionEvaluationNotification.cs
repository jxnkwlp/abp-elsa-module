using Elsa.Services.Models;
using MediatR;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

public class CSharpExpressionEvaluationNotification : INotification
{
    public CSharpExpressionEvaluationNotification(string programText, CSharpEvaluationContext context, ActivityExecutionContext activityExecutionContext)
    {
        ProgramText = programText;
        Context = context;
        ActivityExecutionContext = activityExecutionContext;
    }

    public string ProgramText { get; }

    public CSharpEvaluationContext Context { get; }

    public ActivityExecutionContext ActivityExecutionContext { get; }

}
