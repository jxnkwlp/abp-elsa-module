using MediatR;
using Microsoft.CodeAnalysis.Scripting;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

public class CSharpEvaluationConfigureNotification : INotification
{
    public CSharpEvaluationConfigureNotification(string programText, CSharpEvaluationContext context, ScriptOptions scriptOptions)
    {
        ProgramText = programText;
        Context = context;
        ScriptOptions = scriptOptions;
    }

    public string ProgramText { get; }

    public CSharpEvaluationContext Context { get; }

    public ScriptOptions ScriptOptions { get; private set; }

    public void UpdateScriptOptions(ScriptOptions options) => ScriptOptions = options;
}
