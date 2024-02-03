using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptProject : ICSharpScriptProject
{
    private readonly List<Assembly> _references = new List<Assembly>();
    private readonly List<string> _imports = new List<string>();

    private Project _project = null!;

    public string Id { get; }
    public string Name { get; }
    public ProjectId ProjectId { get; }
    protected AdhocWorkspace Workspace { get; }

    public CSharpScriptProject(AdhocWorkspace workspace, ProjectId projectId, string projectName)
    {
        Id = projectId.Id.ToString();
        Name = projectName;
        Workspace = workspace;
        ProjectId = projectId;
        ReloadProject();
    }

    public void ReloadProject()
    {
        _project = Workspace.CurrentSolution.GetProject(ProjectId)!;
    }

    public IReadOnlyList<string> GetImports()
    {
        return _imports;
    }

    public IReadOnlyList<Assembly> GetReferences()
    {
        return _references;
    }

    public ICSharpScriptProject AddReferences(IEnumerable<Assembly> assemblies)
    {
        _references.AddRange(assemblies);

        _project = _project.AddMetadataReferences(assemblies.Select(x => MetadataReference.CreateFromFile(x.Location)));

        return this;
    }

    public ICSharpScriptProject AddImports(IEnumerable<string> usings)
    {
        _imports.AddRange(usings);

        return this;
    }

    public Document CreateOrUpdateDocument(string fileName, string sourceText)
    {
        lock (fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);

            var document = _project.Documents.FirstOrDefault(x => x.Name == name);

            if (document != null)
            {
                _ = Workspace.CurrentSolution.RemoveDocument(document.Id);
            }

            document = Workspace.AddDocument(_project.Id, name, SourceText.From(sourceText, Encoding.UTF8));

            Workspace.TryApplyChanges(Workspace.CurrentSolution);

            ReloadProject();

            return document;
        }
    }

    public async Task SaveAsync()
    {
        var root = _project.FilePath;

        if (string.IsNullOrWhiteSpace(root))
        {
            return;
        }

        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }

        foreach (var document in _project.Documents)
        {
            var file = Path.Combine(root, document.FilePath ?? document.Name + ".cs");
            var text = await document.GetTextAsync();
            await File.WriteAllTextAsync(file, text?.ToString());
        }
        // TODO
        // project file ?
    }
}
