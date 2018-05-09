using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3047_ValueConvertersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3047";

        public MiKo_3047_ValueConvertersAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            if (symbol.IsValueConverter() || symbol.IsMultiValueConverter())
            {
                if (symbol.ContainingNamespace?.Name != "Converters")
                {
                    return new[] { ReportIssue(symbol) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}