using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Workflow;

[Area(ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[RemoteService(Name = ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[Route("api/elsa/designer")]
public class DesignerController : ElsaModuleController, IDesignerAppService
{
    private readonly IDesignerAppService _service;

    public DesignerController(IDesignerAppService service)
    {
        _service = service;
    }

    [HttpGet("activity-types")]
    public Task<ActivityTypeDescriptorListResultDto> GetActivityTypesAsync()
    {
        return _service.GetActivityTypesAsync();
    }

    [HttpPost("runtime-select-list")]
    public Task<RuntimeSelectListResultDto> GetRuntimeSelectListItems(RuntimeSelectListContextDto input)
    {
        return _service.GetRuntimeSelectListItems(input);
    }

    [HttpGet("scripting/javascript/type-definitions/{id}")]
    public Task<IRemoteStreamContent> GetJavaScriptTypeDefinitionAsync(Guid id)
    {
        return _service.GetJavaScriptTypeDefinitionAsync(id);
    }

    [HttpPost("scripting/csharp/completions/{id}")]
    public Task<WorkflowDesignerCSharpLanguageCompletionProviderResultDto> CSharpLanguageCompletionProviderAsync(Guid id, WorkflowDesignerCSharpLanguageCompletionProviderRequestDto input)
    {
        return _service.CSharpLanguageCompletionProviderAsync(id, input);
    }

    [HttpPost("scripting/csharp/hovers/{id}")]
    public Task<WorkflowDesignerCSharpLanguageHoverProviderResultDto> CSharpLanguageHoverProviderAsync(Guid id, WorkflowDesignerCSharpLanguageHoverProviderRequestDto input)
    {
        return _service.CSharpLanguageHoverProviderAsync(id, input);
    }

    [HttpPost("scripting/csharp/signatures/{id}")]
    public Task<WorkflowDesignerCSharpLanguageSignatureProviderResultDto> CSharpLanguageSignatureProviderAsync(Guid id, WorkflowDesignerCSharpLanguageSignatureProviderRequestDto input)
    {
        return _service.CSharpLanguageSignatureProviderAsync(id, input);
    }

    [HttpPost("scripting/csharp/analysis/{id}")]
    public Task<WorkflowDesignerCSharpLanguageAnalysisResultDto> CSharpLanguageCodeAnalysisAsync(Guid id, WorkflowDesignerCSharpLanguageAnalysisRequestDto input)
    {
        return _service.CSharpLanguageCodeAnalysisAsync(id, input);
    }

    [HttpPost("scripting/csharp/format")]
    public Task<WorkflowDesignerCSharpLanguageFormatterResult> CSharpLanguageCodeFormatterAsync(WorkflowDesignerCSharpLanguageFormatterRequestDto input)
    {
        return _service.CSharpLanguageCodeFormatterAsync(input);
    }
}
