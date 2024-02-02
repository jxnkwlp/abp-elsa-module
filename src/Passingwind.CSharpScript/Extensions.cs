using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Passingwind.CSharpScriptEngine;

public static class Extensions
{
    public static SyntaxTree RemoveReferenceDirectives(this SyntaxTree syntaxTree, ParseOptions parseOptions, CancellationToken cancellationToken = default)
    {
        var syntaxRoot = syntaxTree.GetRoot(cancellationToken);

        var compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot(cancellationToken: cancellationToken);

        if (compilationUnitSyntax.ContainsDirectives)
        {
            var referenceDirectiveNodes = syntaxTree.GetCompilationUnitRoot(cancellationToken: cancellationToken).GetReferenceDirectives();
            syntaxRoot = syntaxRoot.RemoveNodes(referenceDirectiveNodes, SyntaxRemoveOptions.KeepExteriorTrivia);
            return syntaxTree.WithRootAndOptions(syntaxRoot!, parseOptions);
        }

        return syntaxTree;
    }
}
