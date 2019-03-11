using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1030_BaseTypePrefixSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1030";

        public MiKo_1030_BaseTypePrefixSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            foreach (var marker in Constants.Markers.BaseClasses)
            {
                if (symbol.Name.Contains(marker))
                    yield return ReportIssue(symbol, marker);
            }
        }
    }
}