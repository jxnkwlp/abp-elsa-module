using Passingwind.Abp.ElsaModule.CSharp;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageSignatureProviderResultDto
{
    public MonacoSignatures[] Signatures { get; set; }
    public int ActiveParameter { get; set; }
    public int ActiveSignature { get; set; }
}
