using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1034_FieldModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1034";

        public MiKo_1034_FieldModelSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        internal static string FindBetterName(IFieldSymbol symbol) => FindBetterNameForEntityMarker(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeEntityMarkers(symbol);
    }
}