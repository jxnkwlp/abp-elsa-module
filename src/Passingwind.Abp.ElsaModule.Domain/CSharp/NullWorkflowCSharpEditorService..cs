using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class NullMonacoEditorService : IWorkflowCSharpEditorService
{
    public Task<WorkflowCSharpEditorCodeAnalysisResult> GetCodeAnalysisAsync(WorkflowDefinition workflowDefinition, string textId, string text, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<WorkflowCSharpEditorCodeAnalysisResult>(null);
    }

    public Task<WorkflowCSharpEditorFormatterResult> CodeFormatterAsync(string textId, string text, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<WorkflowCSharpEditorFormatterResult>(null);
    }

    public Task<WorkflowCSharpEditorCompletionResult> GetCompletionAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<WorkflowCSharpEditorCompletionResult>(null);
    }

    public Task<WorkflowCSharpEditorHoverInfoResult> GetHoverInfoAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<WorkflowCSharpEditorHoverInfoResult>(null);
    }

    public Task<WorkflowCSharpEditorSignatureResult> GetSignaturesAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<WorkflowCSharpEditorSignatureResult>(null);
    }
}
