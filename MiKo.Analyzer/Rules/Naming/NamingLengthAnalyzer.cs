using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingLengthAnalyzer : NamingAnalyzer
    {
        private readonly int m_limit;

        protected NamingLengthAnalyzer(string diagnosticId, SymbolKind kind, int limit) : base(diagnosticId, kind) => m_limit = limit;

        public static bool EnabledPerDefault { get; set; } = false; // TODO: RKN set to false to limit the default analyzing (but be aware to get the tests running)

        protected override bool IsEnabledByDefault => EnabledPerDefault;

        protected IEnumerable<Diagnostic> Analyze(ISymbol symbol)
        {
            if (symbol.IsOverride)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var exceeding = GetExceedingCharacters(symbol.Name);
            return exceeding > 0
                       ? new[] { Issue(symbol, exceeding) }
                       : Enumerable.Empty<Diagnostic>();
        }

        protected Diagnostic Issue(ISymbol symbol, int exceeding) => Issue(symbol, exceeding, m_limit);

        protected int GetExceedingCharacters(string symbolName)
        {
            var length = symbolName.GetNameOnlyPart().Length;
            return length - m_limit;
        }
    }
}