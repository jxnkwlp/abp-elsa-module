using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageSignatureProviderRequestDto
{ 
    public string SessionId { get; set; }
    public string Code { get; set; }
    public int Position { get; set; }
}
