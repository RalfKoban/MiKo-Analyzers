using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityAnalyzer : Analyzer
    {
        protected MaintainabilityAnalyzer(string diagnosticId) : base(nameof(Maintainability), diagnosticId, SymbolKind.Method)
        {
        }
    }
}