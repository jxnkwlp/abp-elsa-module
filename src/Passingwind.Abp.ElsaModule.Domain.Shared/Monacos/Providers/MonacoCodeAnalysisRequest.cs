using Elsa.Models;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;

public class MonacoCodeAnalysisRequest
{
    public MonacoCodeAnalysisRequest(WorkflowDefinition workflowDefinition, string sessionId, string code)
    {
        WorkflowDefinition = workflowDefinition;
        SessionId = sessionId;
        Code = code;
    }

    public WorkflowDefinition WorkflowDefinition { get; }
    public string SessionId { get; }
    public string Code { get; set; }
}
