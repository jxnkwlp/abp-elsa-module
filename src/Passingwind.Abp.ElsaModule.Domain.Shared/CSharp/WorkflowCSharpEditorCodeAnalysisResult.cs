using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class WorkflowCSharpEditorCodeAnalysisResult
{
    public List<WorkflowCSharpEditorCodeAnalysis> Items { get; set; }

    public WorkflowCSharpEditorCodeAnalysisResult()
    {
        Items = new List<WorkflowCSharpEditorCodeAnalysis>();
    }
}

public class WorkflowCSharpEditorCodeAnalysis
{
    public string Id { get; set; }

    public string Message { get; set; }

    public int OffsetFrom { get; set; }

    public int OffsetTo { get; set; }

    public WorkflowCSharpEditorCodeAnalysisSeverity Severity { get; set; }

    public int SeverityNumeric { get; set; }
}

public enum WorkflowCSharpEditorCodeAnalysisSeverity
{
    Unkown = 0,
    Hint = 1,
    Info = 2,
    Warning = 4,
    Error = 8
}
