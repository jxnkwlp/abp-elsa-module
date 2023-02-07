using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Scripting.CSharp;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;

public class CompletionProvider : IMonacoCompletionProvider
{
    private readonly ILogger<CompletionProvider> _logger;
    private readonly ICSharpSourceGenerator _iCSharpSourceGenerator;

    public CompletionProvider(ILogger<CompletionProvider> logger, ICSharpSourceGenerator iCSharpSourceGenerator)
    {
        _logger = logger;
        _iCSharpSourceGenerator = iCSharpSourceGenerator;
    }

    public async Task<MonacoCompletionResult> HandleAsync(MonacoCompletionRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.SessionId is null) throw new ArgumentNullException(nameof(request.SessionId));

        var generate = await _iCSharpSourceGenerator.GenerateAsync(request.WorkflowDefinition);

        var host = RoslynHostSessionContainer.GetOrCreate(request.SessionId ?? request.WorkflowDefinition.DefinitionId, new RoslynHostReference(generate.Assemblies, generate.Imports));
        host.AddImports(generate.Imports.ToArray());
        host.AddOrUpdateDocument("Definition", generate.Code);
        host.AddOrUpdateScriptDocument(request.Code);

        var document = host.GetScriptDocument();

        var completionService = CompletionService.GetService(document);

        if (completionService == null)
        {
            return new MonacoCompletionResult();
        }

        var completionList = await completionService.GetCompletionsAsync(document, request.Position, CompletionTrigger.Invoke, cancellationToken: cancellationToken);

        var results = new List<MonacoCompletionItem>();

        if (completionList != null)
        {
            for (int i = 0; i < completionList.ItemsList.Count; i++)
            {
                var item = completionList.ItemsList[i];

                SymbolKind symbolKind = SymbolKind.Local;
                if (item.Properties.TryGetValue(nameof(SymbolKind), out var kindValue))
                {
                    symbolKind = Enum.Parse<SymbolKind>(kindValue);
                }

                var completionDescription = await completionService.GetDescriptionAsync(document, completionList.ItemsList[i], cancellationToken);

                results.Add(new MonacoCompletionItem
                {
                    Description = completionDescription.Text,
                    Suggestion = completionList.ItemsList[i].DisplayText,
                    SymbolKind = symbolKind.ToString(),
                    ItemKind = MapKind(symbolKind),
                });
            }
        }

        return new MonacoCompletionResult(results);


        MonacoCompletionItemKind MapKind(SymbolKind symbolKind)
        {
            switch (symbolKind)
            {
                case SymbolKind.Field:
                    {
                        return MonacoCompletionItemKind.Field;
                    }

                case SymbolKind.Property:
                    {
                        return MonacoCompletionItemKind.Property;
                    }

                case SymbolKind.Local:
                    {
                        return MonacoCompletionItemKind.Variable;
                    }

                case SymbolKind.Method:
                    {
                        return MonacoCompletionItemKind.Function;
                    }

                case SymbolKind.NamedType:
                    {
                        return MonacoCompletionItemKind.Class;
                    }

                default:
                    {
                        return MonacoCompletionItemKind.Others;
                    }
            }
        }
    }
}
