using Microsoft.CodeAnalysis.Scripting;

namespace Passingwind.Abp.ElsaModule.Services
{
    public class CSharpEvaluationContext
    {
        public CSharpEvaluationContext(CSharpScriptOptions scriptOptions, ScriptOptions engineOptions, CSharpEvaluationGlobal evaluationGlobal)
        {
            ScriptOptions = scriptOptions;
            EngineOptions = engineOptions;
            EvaluationGlobal = evaluationGlobal;
        }

        public CSharpScriptOptions ScriptOptions { get; set; }
        public Microsoft.CodeAnalysis.Scripting.ScriptOptions EngineOptions { get; }
        public CSharpEvaluationGlobal EvaluationGlobal { get; }
    }
}
