using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class WorkflowCSharpEditorCompletionResult
{
    public List<WorkflowCSharpEditorCompletionItem> Items { get; set; }

    public WorkflowCSharpEditorCompletionResult()
    {
        Items = new List<WorkflowCSharpEditorCompletionItem>();
    }

    public WorkflowCSharpEditorCompletionResult(List<WorkflowCSharpEditorCompletionItem> items)
    {
        Items = items;
    }
}

public class WorkflowCSharpEditorCompletionItem
{
    public string Suggestion { get; set; }
    public string Description { get; set; }
    public string SymbolKind { get; set; }
    public WorkflowCSharpEditorCompletionItemKind ItemKind { get; set; }
}

public enum WorkflowCSharpEditorCompletionItemKind
{
    Function = 0,
    Class = 1,
    Field = 2,
    Variable = 3,
    Property = 4,
    Enum = 5,
    Others = 6,
}
