using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageHoverProviderRequestDto
{
    [Required]
    public string Id { get; set; } 
    public string Text { get; set; }
    public int Position { get; set; }
}
