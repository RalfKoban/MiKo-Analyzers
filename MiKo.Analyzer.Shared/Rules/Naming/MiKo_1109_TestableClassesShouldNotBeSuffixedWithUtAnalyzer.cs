using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1109";

        private const string Prefix = "Testable";
        private const string Suffix = "Ut";

        public MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name.AsSpan();

            if (symbolName.EndsWith(Suffix, StringComparison.Ordinal))
            {
                var betterName = FindBetterName(symbolName);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static string FindBetterName(ReadOnlySpan<char> symbolName) => symbolName.StartsWith(Prefix, StringComparison.Ordinal)
                                                                               ? symbolName.WithoutSuffix(Suffix).ToString()
                                                                               : Prefix.ConcatenatedWith(symbolName.WithoutSuffix(Suffix));
    }
}