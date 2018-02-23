using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1032_MethodModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1032";

        public MiKo_1032_MethodModelSuffixAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => symbol.IsSpecialAccessor() ? Enumerable.Empty<Diagnostic>() : AnalyzeEntityMarkers(symbol);
    }
}