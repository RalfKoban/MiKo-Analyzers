using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SpacingAnalyzer : Analyzer
    {
        protected SpacingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Spacing), diagnosticId, kind)
        {
        }
    }
}