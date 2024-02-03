using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;

namespace Passingwind.CSharpScriptEngine;

public interface ICSharpScriptProject
{
    string Id { get; }
    string Name { get; }

    ProjectId ProjectId { get; }

    void ReloadProject();

    Document CreateOrUpdateDocument(string fileName, string sourceText);

    ICSharpScriptProject AddReferences(IEnumerable<Assembly> assemblies);
    ICSharpScriptProject AddImports(IEnumerable<string> usings);

    IReadOnlyList<string> GetImports();
    IReadOnlyList<Assembly> GetReferences();

    Task SaveAsync();
}
