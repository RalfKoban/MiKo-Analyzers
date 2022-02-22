using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3048_ValueConverterHasAttributeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3048";

        public MiKo_3048_ValueConverterHasAttributeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsValueConverter();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.HasAttributeApplied("System.Windows.Data.ValueConversionAttribute") is false)
            {
                yield return Issue(symbol);
            }
        }
    }
}