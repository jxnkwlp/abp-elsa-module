using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;

namespace Passingwind.CSharpScriptEngine;

public interface ICSharpScriptProject
{
    string Id { get; }
    string Name { get; }

    Project Project { get; }

    void Refresh();

    Document CreateOrUpdateDocument(string fileName, string sourceText);

    Task<IReadOnlyList<CompletionItem>> GetCompletionsAsync(Document document, int position);

    void AddReferences(IEnumerable<Assembly> assemblies);
    void AddImports(IEnumerable<string> usings);

    IReadOnlyList<string> GetImports();
    IReadOnlyList<Assembly> GetReferences();
}
