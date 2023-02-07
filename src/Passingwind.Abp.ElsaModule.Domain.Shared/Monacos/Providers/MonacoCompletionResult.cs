using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;

public class MonacoCompletionResult
{
    public List<MonacoCompletionItem> Items { get; set; }

    public MonacoCompletionResult()
    {
        Items = new List<MonacoCompletionItem>();
    }

    public MonacoCompletionResult(List<MonacoCompletionItem> items)
    {
        Items = items;
    }
}

public class MonacoCompletionItem
{
    public string Suggestion { get; set; }
    public string Description { get; set; }
    public string SymbolKind { get; set; }
    public MonacoCompletionItemKind ItemKind { get; set; }
}

public enum MonacoCompletionItemKind
{
    Function,
    Class,
    Field,
    Variable,
    Property,
    Enum,
    Others,
}