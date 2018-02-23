using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1031_TypeModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1031";

        public MiKo_1031_TypeModelSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => AnalyzeEntityMarkers(symbol);
//
//        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.Name.IsEntityMarker()
//                                                                                               ? new[] { ReportIssue(symbol, symbol.Name.RemoveAll(Constants.EntityMarkers)) }
//                                                                                               : Enumerable.Empty<Diagnostic>();
    }
}