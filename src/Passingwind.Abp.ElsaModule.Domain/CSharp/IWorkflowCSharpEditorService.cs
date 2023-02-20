using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public interface IWorkflowCSharpEditorService : ITransientDependency
{
    Task<WorkflowCSharpEditorCodeAnalysisResult> GetCodeAnalysisAsync(WorkflowDefinition workflowDefinition, string textId, string text, CancellationToken cancellationToken = default);
    Task<WorkflowCSharpEditorFormatterResult> CodeFormatterAsync(string textId, string text, CancellationToken cancellationToken = default);
    Task<WorkflowCSharpEditorCompletionResult> GetCompletionAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default);
    Task<WorkflowCSharpEditorHoverInfoResult> GetHoverInfoAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default);
    Task<WorkflowCSharpEditorSignatureResult> GetSignaturesAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default);
}
