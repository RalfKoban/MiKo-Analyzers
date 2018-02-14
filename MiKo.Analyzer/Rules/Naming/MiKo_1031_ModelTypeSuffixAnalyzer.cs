using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1031_ModelTypeSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1031";

        public MiKo_1031_ModelTypeSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.Name.IsEntityMarker()
                                                                                               ? new[] { ReportIssue(symbol, symbol.Name.Substring(0, symbol.Name.Length - 5)) }
                                                                                               : Enumerable.Empty<Diagnostic>();
    }
}