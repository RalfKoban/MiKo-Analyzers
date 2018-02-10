using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingAnalyzer : Analyzer
    {
        protected NamingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method, bool isEnabledByDefault = true) : base(nameof(Naming), diagnosticId, kind, isEnabledByDefault)
        {
        }
    }
}