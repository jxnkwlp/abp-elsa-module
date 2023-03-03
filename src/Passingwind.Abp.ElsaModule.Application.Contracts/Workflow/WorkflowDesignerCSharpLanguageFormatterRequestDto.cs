using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageFormatterRequestDto
{
    [Required]
    public string Id { get; set; }
    public string Text { get; set; }
}
