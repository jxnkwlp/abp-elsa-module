using System.Collections.Concurrent;
using System.Composition;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns.WorkspaceServices;

[Export(typeof(IDocumentationProviderService)), Shared]
internal sealed class DocumentationProviderService : IDocumentationProviderService
{
    private readonly ConcurrentDictionary<string, DocumentationProvider> _assemblyPathToDocumentationProviderMap = new();

    public DocumentationProvider GetDocumentationProvider(string location)
    {
        string finalPath = Path.ChangeExtension(location, "xml");

        return _assemblyPathToDocumentationProviderMap.GetOrAdd(location, _ =>
        {
            if (!File.Exists(finalPath))
            {
                return null;
            }

            return XmlDocumentationProvider.CreateFromFile(finalPath);
        });
    }
}