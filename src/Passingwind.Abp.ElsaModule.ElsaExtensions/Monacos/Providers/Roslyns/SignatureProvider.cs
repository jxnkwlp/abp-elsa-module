using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Scripting.CSharp;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;

public class SignatureProvider : IMonacoSignatureProvider
{
    private readonly ILogger<SignatureProvider> _logger;
    private readonly ICSharpSourceGenerator _iCSharpSourceGenerator;

    public SignatureProvider(ILogger<SignatureProvider> logger, ICSharpSourceGenerator iCSharpSourceGenerator)
    {
        _logger = logger;
        _iCSharpSourceGenerator = iCSharpSourceGenerator;
    }

    public async Task<MonacoSignatureResult> HandleAsync(MonacoSignatureRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.SessionId is null) throw new ArgumentNullException(nameof(request.SessionId));

        var generate = await _iCSharpSourceGenerator.GenerateAsync(request.WorkflowDefinition);

        var host = RoslynHostSessionContainer.GetOrCreate(request.SessionId ?? request.WorkflowDefinition.DefinitionId, new RoslynHostReference(generate.Assemblies, generate.Imports));
        host.AddImports(generate.Imports.ToArray());
        host.AddOrUpdateDocument("Definition", generate.Code);
        host.AddOrUpdateScriptDocument(request.Code);

        var document = host.GetScriptDocument();

        int position = request.Position;
        var syntaxTree = await document.GetSyntaxTreeAsync();
        var cSharpCompilation = await host.GetCompilationAsync();
        var semanticModel = cSharpCompilation.GetSemanticModel(syntaxTree, true);

        var invocation = await InvocationContext.GetInvocation(document, position);
        if (invocation == null) return null;

        int activeParameter = 0;
        foreach (var comma in invocation.Separators)
        {
            if (comma.Span.Start > invocation.Position)
                break;

            activeParameter += 1;
        }

        var signaturesSet = new HashSet<MonacoSignatures>();
        var bestScore = int.MinValue;
        MonacoSignatures bestScoredItem = null;

        var types = invocation.ArgumentTypes;
        ISymbol throughSymbol = null;
        ISymbol throughType = null;
        var methodGroup = invocation.SemanticModel.GetMemberGroup(invocation.Receiver).OfType<IMethodSymbol>();
        if (invocation.Receiver is MemberAccessExpressionSyntax)
        {
            var throughExpression = ((MemberAccessExpressionSyntax)invocation.Receiver).Expression;
            var typeInfo = semanticModel.GetTypeInfo(throughExpression);
            throughSymbol = invocation.SemanticModel.GetSpeculativeSymbolInfo(invocation.Position, throughExpression, SpeculativeBindingOption.BindAsExpression).Symbol;
            throughType = invocation.SemanticModel.GetSpeculativeTypeInfo(invocation.Position, throughExpression, SpeculativeBindingOption.BindAsTypeOrNamespace).Type;
            var includeInstance = throughSymbol != null && !(throughSymbol is ITypeSymbol) ||
                throughExpression is LiteralExpressionSyntax ||
                throughExpression is TypeOfExpressionSyntax;
            var includeStatic = throughSymbol is INamedTypeSymbol || throughType != null;
            if (throughType == null)
            {
                throughType = typeInfo.Type;
                includeInstance = true;
            }
            methodGroup = methodGroup.Where(m => m.IsStatic && includeStatic || !m.IsStatic && includeInstance);
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

        return new MonacoSignatureResult()
        {
            Signatures = signaturesSet.ToArray(),
            ActiveParameter = activeParameter,
            ActiveSignature = Array.IndexOf(signaturesSet.ToArray(), bestScoredItem)
        };
    }

    private static MonacoSignatures BuildSignature(IMethodSymbol symbol)
    {
        var parameters = new List<MonacoSignatureParameter>();
        foreach (var parameter in symbol.Parameters)
        {
            parameters.Add(new MonacoSignatureParameter() { Label = parameter.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) });
        };
        var signature = new MonacoSignatures
        {
            Documentation = symbol.GetDocumentationCommentXml(),
            Label = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            Parameters = parameters.ToArray()
        };

        return signature;
    }

    private int InvocationScore(IMethodSymbol symbol, IEnumerable<TypeInfo> types)
    {
        var parameters = symbol.Parameters;
        if (parameters.Count() < types.Count())
            return int.MinValue;

        var score = 0;
        var invocationEnum = types.GetEnumerator();
        var definitionEnum = parameters.GetEnumerator();
        while (invocationEnum.MoveNext() && definitionEnum.MoveNext())
        {
            if (invocationEnum.Current.ConvertedType == null)
                score += 1;

            else if (SymbolEqualityComparer.Default.Equals(invocationEnum.Current.ConvertedType, definitionEnum.Current.Type))
                score += 2;
        }
        return score;
    }

    public class InvocationContext
    {
        public static async Task<InvocationContext> GetInvocation(Document document, int position)
        {
            var sourceText = await document.GetTextAsync();
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
