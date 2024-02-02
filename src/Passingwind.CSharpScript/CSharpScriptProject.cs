using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptProject : ICSharpScriptProject
{
    private readonly List<Assembly> _references = new List<Assembly>();
    private readonly List<string> _imports = new List<string>();

    public string Id { get; }
    public string Name { get; }

    public Project Project { get; private set; }

    protected AdhocWorkspace Workspace { get; }

    public CSharpScriptProject(AdhocWorkspace workspace, Project project)
    {
        Id = project.Id.Id.ToString();
        Name = project.Name;
        Workspace = workspace;
        Project = project;
    }

    public void Refresh()
    {
        Project = Workspace.CurrentSolution.GetProject(Project.Id)!;
    }

    public IReadOnlyList<string> GetImports()
    {
        return _imports;
    }

    public IReadOnlyList<Assembly> GetReferences()
    {
        return _references;
    }

    public void AddReferences(IEnumerable<Assembly> assemblies)
    {
        _references.AddRange(assemblies);
    }

    public void AddImports(IEnumerable<string> usings)
    {
        _imports.AddRange(usings);
    }

    public Document CreateOrUpdateDocument(string fileName, string sourceText)
    {
        lock (fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);

            var document = Project.Documents.FirstOrDefault(x => x.Name == name);

            if (document != null)
            {
                var solution = Workspace.CurrentSolution.WithDocumentText(document.Id, SourceText.From(sourceText, Encoding.UTF8));
                // Workspace.TryApplyChanges(solution);
            }
            else
            {
                document = Workspace.AddDocument(Project.Id, name, SourceText.From(sourceText));
                Workspace.TryApplyChanges(Project.Solution);
            }

            Refresh();

            return document;
        }
    }

    public async Task<IReadOnlyList<CompletionItem>> GetCompletionsAsync(Document document, int position)
    {
        var completionService = CompletionService.GetService(document);

        if (completionService == null)
        {
            return new List<CompletionItem>();
        }

        var completionResult = await completionService.GetCompletionsAsync(document, position, CompletionTrigger.Invoke);

        var text = await document.GetTextAsync();

        var textSpanToText = new Dictionary<TextSpan, string>();

        return completionResult.ItemsList.Where(item => MatchesFilterText(completionService, document, item, text, textSpanToText)).ToList();
    }

    private static bool MatchesFilterText(CompletionService completionService, Document document, CompletionItem item, SourceText text, Dictionary<TextSpan, string> textSpanToText)
    {
        var filterText = GetFilterText(item, text, textSpanToText);
        if (string.IsNullOrEmpty(filterText))
        {
            return true;
        }

        return completionService.FilterItems(document, ImmutableArray.Create(item), filterText).Length > 0;
    }

    private static string GetFilterText(CompletionItem item, SourceText text, Dictionary<TextSpan, string> textSpanToText)
    {
        var textSpan = item.Span;
        if (!textSpanToText.TryGetValue(textSpan, out var filterText))
        {
            filterText = text.GetSubText(textSpan).ToString();
            textSpanToText[textSpan] = filterText;
        }
        return filterText;
    }
}
