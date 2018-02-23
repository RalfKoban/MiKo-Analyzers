using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.Name.Contains("Base")
                                                                                               ? new[] { ReportIssue(symbol, symbol.Name.RemoveAll("Base")) }
                                                                                               : Enumerable.Empty<Diagnostic>();
    }
}