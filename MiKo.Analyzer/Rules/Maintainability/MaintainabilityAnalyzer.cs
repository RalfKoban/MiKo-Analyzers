using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityAnalyzer : Analyzer
    {
        protected MaintainabilityAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Maintainability), diagnosticId, kind)
        {
        }

        protected IMethodSymbol GetEnclosingMethod(SyntaxNode node, SemanticModel semanticModel)
        {
            var method = semanticModel.GetEnclosingSymbol(node.GetLocation().SourceSpan.Start) as IMethodSymbol;
            return method;
        }

        protected MethodDeclarationSyntax GetEnclosingMethodSyntax(SyntaxNode node)
        {
            while (true)
            {
                switch (node)
                {
                    case null: return null;
                    case MethodDeclarationSyntax m: return m;
                }

                node = node.Parent;
            }
        }
    }
}