using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1107_TestableClassesShouldNotBeSuffixedWithUtAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1107";

        public MiKo_1107_TestableClassesShouldNotBeSuffixedWithUtAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith("Ut", StringComparison.Ordinal))
            {
                var newName = "Testable" + symbolName.WithoutSuffix("Ut");
                yield return Issue(symbol, newName);
            }
        }
    }
}