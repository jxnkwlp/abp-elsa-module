namespace Passingwind.Abp.ElsaModule.CSharp;
public class WorkflowCSharpEditorSignatureResult
{
    public MonacoSignatures[] Signatures { get; set; }
    public int ActiveParameter { get; set; }
    public int ActiveSignature { get; set; }
}

public class MonacoSignatures
{
    public string Label { get; set; }

    public string Documentation { get; set; }

    public MonacoSignatureParameter[] Parameters { get; set; }
}

public class MonacoSignatureParameter
{
    public string Label { get; set; }

    public string Documentation { get; set; }
}
