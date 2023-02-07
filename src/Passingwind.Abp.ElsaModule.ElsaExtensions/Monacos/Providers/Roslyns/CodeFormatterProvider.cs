using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Formatting;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;
public class CodeFormatterProvider : IMonacoCodeFormatterProvider
{
    public async Task<MonacoCodeFormatterResult> HandleAsync(MonacoCodeFormatterRequest request, CancellationToken cancellationToken = default)
    {
        var host = RoslynHostSessionContainer.GetOrCreate(Guid.NewGuid().ToString(), null);
        host.AddOrUpdateScriptDocument(request.Code);

        var doc = host.GetScriptDocument();

        var formattedDocument = await Formatter.FormatAsync(doc);

        var source = await formattedDocument.GetTextAsync();

        return new MonacoCodeFormatterResult
        {
            Code = source.ToString(),
        };
    }
}
