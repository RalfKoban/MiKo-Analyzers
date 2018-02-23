using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1004_EventNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1004";

        private const string InvalidSuffix = "Event";

        public MiKo_1004_EventNameSuffixAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol)
        {
            if (!symbol.IsOverride && symbol.Name.EndsWith(InvalidSuffix, StringComparison.Ordinal))
                return new[] { ReportIssue(symbol, symbol.Name.RemoveAll(InvalidSuffix)) };

            return Enumerable.Empty<Diagnostic>();
        }
    }
}