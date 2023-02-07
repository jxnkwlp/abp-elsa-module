using Elsa.Models;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;

public class MonacoCompletionRequest
{
    public MonacoCompletionRequest(WorkflowDefinition workflowDefinition, string sessionId, string code, int position)
    {
        WorkflowDefinition = workflowDefinition;
        SessionId = sessionId;
        Code = code;
        Position = position;
    }

    public WorkflowDefinition WorkflowDefinition { get; }
    public string SessionId { get; }
    public string Code { get; }
    public int Position { get; }

}
