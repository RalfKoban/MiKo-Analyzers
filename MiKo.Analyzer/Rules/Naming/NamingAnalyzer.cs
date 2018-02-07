using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingAnalyzer : Analyzer
    {
        protected NamingAnalyzer(string diagnosticId) : base(nameof(Naming), diagnosticId, SymbolKind.Method)
        {
        }
    }
}