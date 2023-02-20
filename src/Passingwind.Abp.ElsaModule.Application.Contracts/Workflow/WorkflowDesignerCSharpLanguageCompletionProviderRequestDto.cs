using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageCompletionProviderRequestDto
{
    [Required]
    public string Id { get; set; }
    public string Text { get; set; }
    public int Position { get; set; }
}
