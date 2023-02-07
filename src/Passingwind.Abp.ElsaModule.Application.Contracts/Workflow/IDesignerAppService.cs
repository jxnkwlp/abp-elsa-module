using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Workflow;

public interface IDesignerAppService : IApplicationService
{
    Task<ActivityTypeDescriptorListResultDto> GetActivityTypesAsync();

    Task<RuntimeSelectListResultDto> GetRuntimeSelectListItems(RuntimeSelectListContextDto input);

    Task<IRemoteStreamContent> GetJavaScriptTypeDefinitionAsync(Guid id);

    Task<WorkflowDesignerCSharpLanguageCompletionProviderResultDto> CSharpLanguageCompletionProviderAsync(Guid id, WorkflowDesignerCSharpLanguageCompletionProviderRequestDto input);

    Task<WorkflowDesignerCSharpLanguageHoverProviderResultDto> CSharpLanguageHoverProviderAsync(Guid id, WorkflowDesignerCSharpLanguageHoverProviderRequestDto input);

    Task<WorkflowDesignerCSharpLanguageSignatureProviderResultDto> CSharpLanguageSignatureProviderAsync(Guid id, WorkflowDesignerCSharpLanguageSignatureProviderRequestDto input);

    Task<WorkflowDesignerCSharpLanguageAnalysisResultDto> CSharpLanguageCodeAnalysisAsync(Guid id, WorkflowDesignerCSharpLanguageAnalysisRequestDto input);

    Task<WorkflowDesignerCSharpLanguageFormatterResult> CSharpLanguageCodeFormatterAsync(WorkflowDesignerCSharpLanguageFormatterRequestDto input);

}
