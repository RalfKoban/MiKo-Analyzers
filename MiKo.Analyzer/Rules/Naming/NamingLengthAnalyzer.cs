using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingLengthAnalyzer : NamingAnalyzer
    {
        private const bool EnabledPerDefault = false; // TODO: RKN set to false to limit the default analyzing (but be aware to get the tests running)

        private readonly int m_limit;

        protected NamingLengthAnalyzer(string diagnosticId, SymbolKind kind, int limit) : base(diagnosticId, kind, EnabledPerDefault) => m_limit = limit;

        protected IEnumerable<Diagnostic> Analyze(ISymbol symbol)
        {
            if (symbol.IsOverride) return Enumerable.Empty<Diagnostic>();

            var exceeding = symbol.Name.Length - m_limit;
            return exceeding > 0
                       ? new[] { ReportIssue(symbol, exceeding, m_limit) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}