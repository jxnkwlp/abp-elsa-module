using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Monacos.Providers;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageSignatureProviderResultDto
{
    public MonacoSignatures[] Signatures { get; set; }
    public int ActiveParameter { get; set; }
    public int ActiveSignature { get; set; }
}
