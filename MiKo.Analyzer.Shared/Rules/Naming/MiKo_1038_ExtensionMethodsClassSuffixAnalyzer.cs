using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1038_ExtensionMethodsClassSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1038";

        internal const string Suffix = "Extensions";

        private static readonly string[] WrongSuffixes =
            {
                "ExtensionMethods",
                "ExtensionMethod",
                "Extension",
                "ExtensionsClass",
                "ExtensionClass",
            };

        public MiKo_1038_ExtensionMethodsClassSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static string FindBetterName(ITypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            foreach (var wrongSuffix in WrongSuffixes)
            {
                if (symbolName.EndsWith(wrongSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    symbolName = symbolName.WithoutSuffix(wrongSuffix);

                    break;
                }
            }

            return symbolName + Suffix;
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.ContainsExtensionMethods();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { Issue(symbol, Suffix) };
    }
}