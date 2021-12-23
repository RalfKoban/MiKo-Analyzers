using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1035_PropertyModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1035";

        public MiKo_1035_PropertyModelSuffixAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        internal static string FindBetterName(IPropertySymbol symbol) => FindBetterNameForEntityMarker(symbol);

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeEntityMarkers(symbol);
    }
}