using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class OrderingAnalyzer : Analyzer
    {
        protected OrderingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Ordering), diagnosticId, kind)
        {
        }
    }
}