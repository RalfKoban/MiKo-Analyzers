using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingLengthAnalyzer : NamingAnalyzer
    {
        private readonly int m_limit;

        protected NamingLengthAnalyzer(string diagnosticId, SymbolKind kind, int limit) : base(diagnosticId, kind) => m_limit = limit;

        protected IEnumerable<Diagnostic> Analyze(ISymbol symbol)
        {
            var exceeding = symbol.Name.Length - m_limit;
            return exceeding > 0
                       ? new[] { ReportIssue(symbol, exceeding, m_limit) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}