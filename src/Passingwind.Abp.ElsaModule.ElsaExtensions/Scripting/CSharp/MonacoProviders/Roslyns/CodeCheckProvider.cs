//using System;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Options;

//namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoCompletionProviders.Roslyns;

//public class CodeCheckProvider : ICodeTestProvider
//{
//    private readonly ICSharpEvaluator _cSharpEvaluator;
//    private readonly IOptions<CSharpScriptOptions> _options;

//    public CodeCheckProvider(ICSharpEvaluator cSharpEvaluator, IOptions<CSharpScriptOptions> options)
//    {
//        _cSharpEvaluator = cSharpEvaluator;
//        _options = options;
//    }

//    public async Task<CodeCheckResult> HandlerAsync(CodeCheckRequest request)
//    {
//        //var workspace = CompletionWorkspace.Create(codeCheckRequest.Assemblies);
//        //var document = await workspace.CreateDocument(codeCheckRequest.Code);

//        try
//        {
//            CSharpEvaluationGlobal scriptGlobal = new CSharpEvaluationGlobal();
//            CSharpEvaluationContext evaluationContext = new CSharpEvaluationContext(_options.Value, scriptGlobal);

//            await _cSharpEvaluator.TestAsync(request.Code, evaluationContext, (c) => { });
//        }
//        catch (Exception)
//        {
//            throw;
//        }
//    }
//}
