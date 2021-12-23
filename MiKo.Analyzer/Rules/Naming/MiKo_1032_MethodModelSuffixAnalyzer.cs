using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1032_MethodModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1032";

        public MiKo_1032_MethodModelSuffixAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IMethodSymbol symbol) => FindBetterNameForEntityMarker(symbol);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsSpecialAccessor() is false && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeEntityMarkers(symbol);
    }
}