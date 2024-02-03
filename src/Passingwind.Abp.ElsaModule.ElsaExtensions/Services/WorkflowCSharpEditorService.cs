using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.CSharp;
using Passingwind.Abp.ElsaModule.Scripting.CSharp;
using Passingwind.CSharpScriptEngine;

namespace Passingwind.Abp.ElsaModule.Services;

public class WorkflowCSharpEditorService : IWorkflowCSharpEditorService
{
    private const string _generatedTypeClassName = "GeneratedTypes";

    private readonly ILogger<WorkflowCSharpEditorService> _logger;
    private readonly ICSharpTypeDefinitionService _cSharpTypeDefinitionService;
    private readonly ICSharpScriptWorkspace _cSharpScriptWorkspace;

    public WorkflowCSharpEditorService(ILogger<WorkflowCSharpEditorService> logger, ICSharpTypeDefinitionService cSharpTypeDefinitionService, ICSharpScriptWorkspace cSharpScriptWorkspace)
    {
        _logger = logger;
        _cSharpTypeDefinitionService = cSharpTypeDefinitionService;
        _cSharpScriptWorkspace = cSharpScriptWorkspace;
    }

    protected async Task<ICSharpScriptProject> GetProjectAsync(WorkflowDefinition workflowDefinition, CancellationToken cancellationToken = default)
    {
        var generated = await _cSharpTypeDefinitionService.GenerateAsync(workflowDefinition, cancellationToken);

        var projectName = workflowDefinition.DefinitionId.Replace("-", null);

        var project = _cSharpScriptWorkspace.GetOrCreateProject(projectName);

        // add default
        _ = project.CreateOrUpdateDocument(_generatedTypeClassName, generated.Text);

        _ = project.AddImports(generated.Imports)
            .AddReferences(generated.Assemblies);

        return project;
    }

    public async Task<WorkflowCSharpEditorCodeAnalysisResult> GetCodeAnalysisAsync(WorkflowDefinition workflowDefinition, string textId, string text, CancellationToken cancellationToken = default)
    {
        var project = await GetProjectAsync(workflowDefinition, cancellationToken);

        project.CreateOrUpdateDocument(textId, text);

#if DEBUG
        await project.SaveAsync();
#endif

        var tmpFile = Path.GetTempFileName();

        var compilation = await _cSharpScriptWorkspace.CreateCompilationAsync(project, true, cancellationToken);

        var emitResult = compilation.Emit(tmpFile, cancellationToken: cancellationToken);

        var result = new List<WorkflowCSharpEditorCodeAnalysis>();

        if (emitResult?.Success == false)
        {
            foreach (var diagnostic in emitResult.Diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning))
            {
                var severity = MapDiagnosticSeverity(diagnostic);

                var msg = new WorkflowCSharpEditorCodeAnalysis()
                {
                    Id = diagnostic.Id,
                    Message = diagnostic.GetMessage(),
                    OffsetFrom = diagnostic.Location.SourceSpan.Start,
                    OffsetTo = diagnostic.Location.SourceSpan.End,
                    Severity = severity,
                    SeverityNumeric = (int)severity,
                };
                result.Add(msg);
            }
        }

        return new WorkflowCSharpEditorCodeAnalysisResult() { Items = result };
    }

    public async Task<WorkflowCSharpEditorFormatterResult> CodeFormatterAsync(string textId, string text, CancellationToken cancellationToken = default)
    {
        using var workspce = new AdhocWorkspace();

        var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "tmp", "tmp", LanguageNames.CSharp);

        var project = workspce.AddProject(projectInfo);

        var document = workspce.AddDocument(project.Id, "tmp", SourceText.From(text));

        _ = workspce.TryApplyChanges(workspce.CurrentSolution);

        var formattedDocument = await Formatter.FormatAsync(document, cancellationToken: cancellationToken);
        var sourceText = await formattedDocument.GetTextAsync(cancellationToken);

        return new WorkflowCSharpEditorFormatterResult
        {
            Text = sourceText?.ToString()
        };
    }

    public async Task<WorkflowCSharpEditorCompletionResult> GetCompletionAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default)
    {
        var project = await GetProjectAsync(workflowDefinition, cancellationToken);

        if (position > text.Length)
        {
            position = text.Length;
        }

        var document = project.CreateOrUpdateDocument(textId, text);

#if DEBUG
        await project.SaveAsync();
#endif

        await _cSharpScriptWorkspace.RestoreReferenceAsync(project, cancellationToken);

        var completionItems = await _cSharpScriptWorkspace.GetCompletionsAsync(project, document.Id, position, cancellationToken: cancellationToken);

        var result = new List<WorkflowCSharpEditorCompletionItem>();

        foreach (var item in completionItems)
        {
            var symbolKind = SymbolKind.Local;
            if (item.Properties.TryGetValue(nameof(SymbolKind), out var kindValue))
            {
                symbolKind = Enum.Parse<SymbolKind>(kindValue);
            }

            // var completionDescription = await CompletionService.GetDescriptionAsync(document, item, cancellationToken);

            result.Add(new WorkflowCSharpEditorCompletionItem
            {
                // Description = completionDescription.Text,
                Suggestion = item.DisplayText,
                SymbolKind = symbolKind.ToString(),
                ItemKind = MapKind(symbolKind),
            });
        }

        return new WorkflowCSharpEditorCompletionResult(result);

        static WorkflowCSharpEditorCompletionItemKind MapKind(SymbolKind symbolKind)
        {
            return symbolKind switch
            {
                SymbolKind.Field => WorkflowCSharpEditorCompletionItemKind.Field,
                SymbolKind.Property => WorkflowCSharpEditorCompletionItemKind.Property,
                SymbolKind.Local => WorkflowCSharpEditorCompletionItemKind.Variable,
                SymbolKind.Method => WorkflowCSharpEditorCompletionItemKind.Function,
                SymbolKind.NamedType => WorkflowCSharpEditorCompletionItemKind.Class,
                _ => WorkflowCSharpEditorCompletionItemKind.Others,
            };
        }
    }

    public async Task<WorkflowCSharpEditorHoverInfoResult> GetHoverInfoAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default)
    {
        var project = await GetProjectAsync(workflowDefinition, cancellationToken);

        var document = project.CreateOrUpdateDocument(textId, text);

#if DEBUG
        await project.SaveAsync();
#endif

        await _cSharpScriptWorkspace.RestoreReferenceAsync(project, cancellationToken);

        var compilation = await _cSharpScriptWorkspace.CreateCompilationAsync(project, false, cancellationToken);

        var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);

        var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
        var expressionNode = syntaxRoot.FindToken(position).Parent;

        TypeInfo typeInfo;
        if (expressionNode is VariableDeclaratorSyntax)
        {
            var childNode = expressionNode.ChildNodes()?.FirstOrDefault()?.ChildNodes()?.FirstOrDefault();
            if (childNode != null)
            {
                typeInfo = semanticModel.GetTypeInfo(childNode, cancellationToken: cancellationToken);
                var location = expressionNode.GetLocation();
                if (typeInfo.Type != null)
                {
                    return new WorkflowCSharpEditorHoverInfoResult()
                    {
                        Information = typeInfo.Type.ToString(),
                        OffsetFrom = location.SourceSpan.Start,
                        OffsetTo = location.SourceSpan.End
                    };
                }
            }
        }

        if (expressionNode is PropertyDeclarationSyntax prop)
        {
            var location = expressionNode.GetLocation();
            return new WorkflowCSharpEditorHoverInfoResult()
            {
                Information = prop.Type.ToString(),
                OffsetFrom = location.SourceSpan.Start,
                OffsetTo = location.SourceSpan.End
            };
        }

        if (expressionNode is ParameterSyntax p)
        {
            var location = expressionNode.GetLocation();
            return new WorkflowCSharpEditorHoverInfoResult()
            {
                Information = p.Type.ToString(),
                OffsetFrom = location.SourceSpan.Start,
                OffsetTo = location.SourceSpan.End
            };
        }

        if (expressionNode is IdentifierNameSyntax i)
        {
            var location = expressionNode.GetLocation();
            typeInfo = semanticModel.GetTypeInfo(i, cancellationToken: cancellationToken);
            if (typeInfo.Type != null)
            {
                return new WorkflowCSharpEditorHoverInfoResult()
                {
                    Information = typeInfo.Type.ToString(),
                    OffsetFrom = location.SourceSpan.Start,
                    OffsetTo = location.SourceSpan.End
                };
            }
        }

        var symbolInfo = semanticModel.GetSymbolInfo(expressionNode, cancellationToken: cancellationToken);
        if (symbolInfo.Symbol != null)
        {
            var location = expressionNode.GetLocation();
            return new WorkflowCSharpEditorHoverInfoResult()
            {
                Information = HoverInfoBuild(symbolInfo),
                OffsetFrom = location.SourceSpan.Start,
                OffsetTo = location.SourceSpan.End
            };
        }

        return null;
    }

    public async Task<WorkflowCSharpEditorSignatureResult> GetSignaturesAsync(WorkflowDefinition workflowDefinition, string textId, string text, int position, CancellationToken cancellationToken = default)
    {
        var project = await GetProjectAsync(workflowDefinition, cancellationToken);

        var document = project.CreateOrUpdateDocument(textId, text);

#if DEBUG
        await project.SaveAsync();
#endif
        await _cSharpScriptWorkspace.RestoreReferenceAsync(project, cancellationToken);

        var compilation = await _cSharpScriptWorkspace.CreateCompilationAsync(project, false, cancellationToken);

        var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
        var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

        var invocation = await InvocationContext.GetInvocation(document, position);
        if (invocation == null)
        {
            return null;
        }

        var activeParameter = 0;
        foreach (var comma in invocation.Separators)
        {
            if (comma.Span.Start > invocation.Position)
            {
                break;
            }

            activeParameter++;
        }

        var signaturesSet = new HashSet<MonacoSignatures>();
        var bestScore = int.MinValue;
        MonacoSignatures bestScoredItem = null;

        var types = invocation.ArgumentTypes;
        ISymbol throughSymbol = null;
        ISymbol throughType = null;
        var methodGroup = invocation.SemanticModel.GetMemberGroup(invocation.Receiver, cancellationToken: cancellationToken).OfType<IMethodSymbol>();
        if (invocation.Receiver is MemberAccessExpressionSyntax)
        {
            var throughExpression = ((MemberAccessExpressionSyntax)invocation.Receiver).Expression;
            var typeInfo = semanticModel.GetTypeInfo(throughExpression, cancellationToken: cancellationToken);
            throughSymbol = invocation.SemanticModel.GetSpeculativeSymbolInfo(invocation.Position, throughExpression, SpeculativeBindingOption.BindAsExpression).Symbol;
            throughType = invocation.SemanticModel.GetSpeculativeTypeInfo(invocation.Position, throughExpression, SpeculativeBindingOption.BindAsTypeOrNamespace).Type;
            var includeInstance = (throughSymbol != null && throughSymbol is not ITypeSymbol) ||
                throughExpression is LiteralExpressionSyntax ||
                throughExpression is TypeOfExpressionSyntax;
            var includeStatic = throughSymbol is INamedTypeSymbol || throughType != null;
            if (throughType == null)
            {
                throughType = typeInfo.Type;
                includeInstance = true;
            }
            methodGroup = methodGroup.Where(m => (m.IsStatic && includeStatic) || (!m.IsStatic && includeInstance));
        }
        else if (invocation.Receiver is SimpleNameSyntax && invocation.IsInStaticContext)
        {
            methodGroup = methodGroup.Where(m => m.IsStatic || m.MethodKind == MethodKind.LocalFunction);
        }

        foreach (var methodOverload in methodGroup)
        {
            var signature = BuildSignature(methodOverload);
            signaturesSet.Add(signature);

            var score = InvocationScore(methodOverload, types);
            if (score > bestScore)
            {
                bestScore = score;
                bestScoredItem = signature;
            }
        }

        return new WorkflowCSharpEditorSignatureResult()
        {
            Signatures = signaturesSet.ToArray(),
            ActiveParameter = activeParameter,
            ActiveSignature = Array.IndexOf(signaturesSet.ToArray(), bestScoredItem)
        };
    }

    private static WorkflowCSharpEditorCodeAnalysisSeverity MapDiagnosticSeverity(Diagnostic diagnostic)
    {
        return diagnostic.Severity switch
        {
            DiagnosticSeverity.Error => WorkflowCSharpEditorCodeAnalysisSeverity.Error,
            DiagnosticSeverity.Warning => WorkflowCSharpEditorCodeAnalysisSeverity.Warning,
            DiagnosticSeverity.Info => WorkflowCSharpEditorCodeAnalysisSeverity.Info,
            _ => WorkflowCSharpEditorCodeAnalysisSeverity.Hint
        };
    }

    private static string HoverInfoBuild(SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol is IMethodSymbol methodsymbol)
        {
            var sb = new StringBuilder().Append("(method) ").Append(methodsymbol.DeclaredAccessibility.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture)).Append(' ');
            if (methodsymbol.IsStatic)
            {
                sb = sb.Append("static").Append(' ');
            }

            sb = sb.Append(methodsymbol.Name).Append('(');
            for (var i = 0; i < methodsymbol.Parameters.Length; i++)
            {
                sb = sb.Append(methodsymbol.Parameters[i].Type).Append(' ').Append(methodsymbol.Parameters[i].Name);
                if (i < methodsymbol.Parameters.Length - 1)
                {
                    sb = sb.Append(", ");
                }
            }
            sb = sb.Append(") : ").Append(methodsymbol.ReturnType);
            return sb.ToString();
        }
        if (symbolInfo.Symbol is ILocalSymbol localsymbol)
        {
            var sb = new StringBuilder().Append(localsymbol.Name).Append(" : ");
            if (localsymbol.IsConst)
            {
                sb = sb.Append("const").Append(' ');
            }

            sb = sb.Append(localsymbol.Type);
            return sb.ToString();
        }
        if (symbolInfo.Symbol is IFieldSymbol fieldSymbol)
        {
            var sb = new StringBuilder().Append(fieldSymbol.Name).Append(" : ").Append(fieldSymbol.DeclaredAccessibility.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture)).Append(' ');
            if (fieldSymbol.IsStatic)
            {
                sb = sb.Append("static").Append(' ');
            }

            if (fieldSymbol.IsReadOnly)
            {
                sb = sb.Append("readonly").Append(' ');
            }

            if (fieldSymbol.IsConst)
            {
                sb = sb.Append("const").Append(' ');
            }

            sb = sb.Append(fieldSymbol.Type);
            return sb.ToString();
        }

        return string.Empty;
    }

    private static MonacoSignatures BuildSignature(IMethodSymbol symbol)
    {
        var parameters = new List<MonacoSignatureParameter>();
        foreach (var parameter in symbol.Parameters)
        {
            parameters.Add(new MonacoSignatureParameter() { Label = parameter.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) });
        }
        return new MonacoSignatures
        {
            Documentation = symbol.GetDocumentationCommentXml(),
            Label = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            Parameters = parameters.ToArray()
        };
    }

    private static int InvocationScore(IMethodSymbol symbol, IEnumerable<TypeInfo> types)
    {
        var parameters = symbol.Parameters;
        if (parameters.Length < types.Count())
        {
            return int.MinValue;
        }

        var score = 0;
        var invocationEnum = types.GetEnumerator();
        var definitionEnum = parameters.GetEnumerator();
        while (invocationEnum.MoveNext() && definitionEnum.MoveNext())
        {
            if (invocationEnum.Current.ConvertedType == null)
            {
                score++;
            }
            else if (SymbolEqualityComparer.Default.Equals(invocationEnum.Current.ConvertedType, definitionEnum.Current.Type))
            {
                score += 2;
            }
        }
        return score;
    }

    public class InvocationContext
    {
        public static async Task<InvocationContext> GetInvocation(Document document, int position)
        {
            _ = await document.GetTextAsync();
            var tree = await document.GetSyntaxTreeAsync();
            var root = await tree.GetRootAsync();
            var node = root.FindToken(position).Parent;

            while (node != null)
            {
                if (node is InvocationExpressionSyntax invocation && invocation.ArgumentList.Span.Contains(position))
                {
                    var semanticModel = await document.GetSemanticModelAsync();
                    return new InvocationContext(semanticModel, position, invocation.Expression, invocation.ArgumentList);
                }

                if (node is BaseObjectCreationExpressionSyntax objectCreation && (objectCreation.ArgumentList?.Span.Contains(position) ?? false))
                {
                    var semanticModel = await document.GetSemanticModelAsync();
                    return new InvocationContext(semanticModel, position, objectCreation, objectCreation.ArgumentList);
                }

                if (node is AttributeSyntax attributeSyntax && (attributeSyntax.ArgumentList?.Span.Contains(position) ?? false))
                {
                    var semanticModel = await document.GetSemanticModelAsync();
                    return new InvocationContext(semanticModel, position, attributeSyntax, attributeSyntax.ArgumentList);
                }

                node = node.Parent;
            }

            return null;
        }

        public SemanticModel SemanticModel { get; }
        public int Position { get; }
        public SyntaxNode Receiver { get; }
        public IEnumerable<TypeInfo> ArgumentTypes { get; }
        public IEnumerable<SyntaxToken> Separators { get; }
        public bool IsInStaticContext { get; }

        public InvocationContext(SemanticModel semModel, int position, SyntaxNode receiver, ArgumentListSyntax argList)
        {
            SemanticModel = semModel;
            Position = position;
            Receiver = receiver;
            ArgumentTypes = argList.Arguments.Select(argument => semModel.GetTypeInfo(argument.Expression));
            Separators = argList.Arguments.GetSeparators();
        }

        public InvocationContext(SemanticModel semModel, int position, SyntaxNode receiver, AttributeArgumentListSyntax argList)
        {
            SemanticModel = semModel;
            Position = position;
            Receiver = receiver;
            ArgumentTypes = argList.Arguments.Select(argument => semModel.GetTypeInfo(argument.Expression));
            Separators = argList.Arguments.GetSeparators();
        }
    }
}
