using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageAnalysisRequestDto
{
    [Required]
    public string Id { get; set; }
    public string Text { get; set; }
}
