namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpEvaluationContext
{
    public CSharpEvaluationContext(CSharpScriptOptions scriptOptions, CSharpEvaluationGlobal evaluationGlobal)
    {
        ScriptOptions = scriptOptions;
        EvaluationGlobal = evaluationGlobal;
    }

    public CSharpScriptOptions ScriptOptions { get; }
    public CSharpEvaluationGlobal EvaluationGlobal { get; }
}
