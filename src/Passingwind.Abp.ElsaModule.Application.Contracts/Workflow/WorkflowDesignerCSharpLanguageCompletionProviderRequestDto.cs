using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageCompletionProviderRequestDto
{ 
    public string SessionId { get; set; }
    public string Code { get; set; }
    public int Position { get; set; }
}
