using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;
public class MonacoCodeAnalysisResult
{
    public List<MonacoCodeAnalysisItem> Items { get; set; }

    public MonacoCodeAnalysisResult()
    {
        Items = new List<MonacoCodeAnalysisItem>();
    }
}

public class MonacoCodeAnalysisItem
{
    public string Id { get; set; }

    public string Keyword { get; set; }

    public string Message { get; set; }

    public int OffsetFrom { get; set; }

    public int OffsetTo { get; set; }

    public MonacoCodeAnalysisSeverity Severity { get; set; }

    public int SeverityNumeric { get; set; }
}

public enum MonacoCodeAnalysisSeverity
{
    Unkown = 0,
    Hint = 1,
    Info = 2,
    Warning = 4,
    Error = 8
}
