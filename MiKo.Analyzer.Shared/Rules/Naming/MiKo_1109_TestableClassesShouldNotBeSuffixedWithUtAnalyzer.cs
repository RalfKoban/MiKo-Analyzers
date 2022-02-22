using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1109";

        public const string Prefix = "Testable";
        public const string Suffix = "Ut";

        public MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(INamedTypeSymbol symbol) => FindBetterName(symbol.Name);

        internal static string FindBetterName(string symbolName) => symbolName.StartsWith(Prefix)
                                                                        ? symbolName.WithoutSuffix(Suffix)
                                                                        : Prefix + symbolName.WithoutSuffix(Suffix);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Suffix, StringComparison.Ordinal))
            {
                var newName = FindBetterName(symbolName);

                yield return Issue(symbol, newName);
            }
        }
    }
}