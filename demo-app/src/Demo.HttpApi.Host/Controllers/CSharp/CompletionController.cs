//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoProviders;
//using Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoProviders.Roslyns;
//using Volo.Abp;

//namespace Passingwind.Abp.ElsaModule.Controllers.CSharp;

//[Authorize]
//[Route("api/_monaco/csharp")]
//[ControllerName("MonacoCsharpCompletion")]
//[RemoteService(false)]
//public class CompletionController : ControllerBase
//{
//    //private readonly ITabCompletionProvider _tabCompletionProvider;
//    //private readonly IHoverInfoProvider _hoverInfoProvider;

//    //public CompletionController(ITabCompletionProvider tabCompletionProvider, IHoverInfoProvider hoverInfoProvider)
//    //{
//    //    _tabCompletionProvider = tabCompletionProvider;
//    //    _hoverInfoProvider = hoverInfoProvider;
//    //}

//    [HttpPost("tabcompletion")]
//    public Task<TabCompletionResult> TabCompletion([FromBody] TabCompletionRequest input)
//    {
//        // return _tabCompletionProvider.HandleAsync(input);
//        return new TabCompletionProvider().HandleAsync(input);
//    }

//    //[HttpPost("hoverinfo")]
//    //public async Task<HoverInfoResult> HoverInfo([FromBody] HoverInfoRequest input)
//    //{
//    //    return await _hoverInfoProvider.HandleAsync(input);
//    //}
//}
