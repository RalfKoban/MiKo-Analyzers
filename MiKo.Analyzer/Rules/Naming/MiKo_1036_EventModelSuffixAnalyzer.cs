using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1036_EventModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1036";

        public MiKo_1036_EventModelSuffixAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol) => AnalyzeEntityMarkers(symbol);
    }
}