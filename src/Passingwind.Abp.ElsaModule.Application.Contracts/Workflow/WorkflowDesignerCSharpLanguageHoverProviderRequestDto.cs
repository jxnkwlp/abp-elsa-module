using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageHoverProviderRequestDto
{
    [Required]
    public string Id { get; set; }
    public string Text { get; set; }
    public int Position { get; set; }
}
