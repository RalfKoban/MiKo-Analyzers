using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SpacingAnalyzer : Analyzer
    {
        protected SpacingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Spacing), diagnosticId, kind)
        {
        }

        protected static LinePosition GetStartPosition(SyntaxNode node) => node.GetLocation().GetLineSpan().StartLinePosition;

        protected static LinePosition GetStartPosition(SyntaxToken token) => token.GetLocation().GetLineSpan().StartLinePosition;
    }
}