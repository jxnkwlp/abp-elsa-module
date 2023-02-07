using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Scripting.CSharp;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;

public class HoverInfoProvider : IMonacoHoverInfoProvider
{
    private readonly ILogger<HoverInfoProvider> _logger;
    private readonly ICSharpSourceGenerator _iCSharpSourceGenerator;

    public HoverInfoProvider(ILogger<HoverInfoProvider> logger, ICSharpSourceGenerator iCSharpSourceGenerator)
    {
        _logger = logger;
        _iCSharpSourceGenerator = iCSharpSourceGenerator;
    }

    public async Task<MonacoHoverInfoResult> HandleAsync(MonacoHoverInfoRequest request, CancellationToken cancellationToken = default)
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

        TypeInfo typeInfo;

        var syntaxTree = await document.GetSyntaxTreeAsync();
        var cSharpCompilation = await host.GetCompilationAsync();
        var semanticModel = cSharpCompilation.GetSemanticModel(syntaxTree, true);

        var syntaxRoot = await document.GetSyntaxRootAsync();
        var expressionNode = syntaxRoot.FindToken(position).Parent;

        if (expressionNode is VariableDeclaratorSyntax)
        {
            SyntaxNode childNode = expressionNode.ChildNodes()?.FirstOrDefault()?.ChildNodes()?.FirstOrDefault();
            if (childNode != null)
            {
                typeInfo = semanticModel.GetTypeInfo(childNode);
                var location = expressionNode.GetLocation();
                if (typeInfo.Type != null)
                    return new MonacoHoverInfoResult()
                    {
                        Information = typeInfo.Type.ToString(),
                        OffsetFrom = location.SourceSpan.Start,
                        OffsetTo = location.SourceSpan.End
                    };
            }
        }

        if (expressionNode is PropertyDeclarationSyntax prop)
        {
            var location = expressionNode.GetLocation();
            return new MonacoHoverInfoResult()
            {
                Information = prop.Type.ToString(),
                OffsetFrom = location.SourceSpan.Start,
                OffsetTo = location.SourceSpan.End
            };
        }

        if (expressionNode is ParameterSyntax p)
        {
            var location = expressionNode.GetLocation();
            return new MonacoHoverInfoResult()
            {
                Information = p.Type.ToString(),
                OffsetFrom = location.SourceSpan.Start,
                OffsetTo = location.SourceSpan.End
            };
        }

        if (expressionNode is IdentifierNameSyntax i)
        {
            var location = expressionNode.GetLocation();
            typeInfo = semanticModel.GetTypeInfo(i);
            if (typeInfo.Type != null)
                return new MonacoHoverInfoResult()
                {
                    Information = typeInfo.Type.ToString(),
                    OffsetFrom = location.SourceSpan.Start,
                    OffsetTo = location.SourceSpan.End
                };
        }

        var symbolInfo = semanticModel.GetSymbolInfo(expressionNode);
        if (symbolInfo.Symbol != null)
        {
            var location = expressionNode.GetLocation();
            return new MonacoHoverInfoResult()
            {
                Information = HoverInfoBuild(symbolInfo),
                OffsetFrom = location.SourceSpan.Start,
                OffsetTo = location.SourceSpan.End
            };
        }

        return null;
    }

    private static string HoverInfoBuild(SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol is IMethodSymbol methodsymbol)
        {
            var sb = new StringBuilder().Append("(method) ").Append(methodsymbol.DeclaredAccessibility.ToString().ToLower()).Append(' ');
            if (methodsymbol.IsStatic)
                sb.Append("static").Append(' ');
            sb.Append(methodsymbol.Name).Append('(');
            for (var i = 0; i < methodsymbol.Parameters.Length; i++)
            {
                sb.Append(methodsymbol.Parameters[i].Type).Append(' ').Append(methodsymbol.Parameters[i].Name);
                if (i < methodsymbol.Parameters.Length - 1) sb.Append(", ");
            }
            sb.Append(") : ");
            sb.Append(methodsymbol.ReturnType).ToString();
            return sb.ToString();
        }
        if (symbolInfo.Symbol is ILocalSymbol localsymbol)
        {
            var sb = new StringBuilder().Append(localsymbol.Name).Append(" : ");
            if (localsymbol.IsConst)
                sb.Append("const").Append(' ');
            sb.Append(localsymbol.Type);
            return sb.ToString();
        }
        if (symbolInfo.Symbol is IFieldSymbol fieldSymbol)
        {
            var sb = new StringBuilder().Append(fieldSymbol.Name).Append(" : ").Append(fieldSymbol.DeclaredAccessibility.ToString().ToLower()).Append(' ');
            if (fieldSymbol.IsStatic)
                sb.Append("static").Append(' ');
            if (fieldSymbol.IsReadOnly)
                sb.Append("readonly").Append(' ');
            if (fieldSymbol.IsConst)
                sb.Append("const").Append(' ');
            sb.Append(fieldSymbol.Type).ToString();
            return sb.ToString();
        }

        return string.Empty;
    }
}
