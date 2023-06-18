using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1090_ParametersWrongSuffixedAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1090";

        // Ordinal is important here as we want to have only those with a suffix and not all (e.g. we want 'comparisonView' but not 'view')
        private const StringComparison Comparison = StringComparison.Ordinal;

        private static readonly string[] Prefixes =
                                                    {
                                                        "old",
                                                        "new",
                                                    };

        private static readonly Dictionary<string, string> WrongSuffixes = new Dictionary<string, string>
                                                                               {
                                                                                   { "Comparer", "comparer" },
                                                                                   { "Editor", "editor" },
                                                                                   { "Item", "item" },
                                                                                   { "View", "view" },
                                                                               };

        public MiKo_1090_ParametersWrongSuffixedAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        internal static string FindBetterName(ISymbol symbol)
        {
            var name = symbol.Name;

            if (name.EndsWith(Constants.Entity, Comparison))
            {
                return name.WithoutSuffix(Constants.Entity);
            }

            if (name.EndsWith("Element", Comparison))
            {
                return name == "frameworkElement"
                       ? "element"
                       : name.WithoutSuffix("Element");
            }

            foreach (var pair in WrongSuffixes)
            {
                if (name.EndsWith(pair.Key, Comparison))
                {
                    return pair.Value;
                }
            }

            return name;
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => ShallAnalyze(symbol.GetEnclosingMethod());

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.Entity, Comparison))
            {
                yield return Issue(symbol, symbolName.WithoutSuffix(Constants.Entity));
            }

            if (symbolName.EndsWith("Element", Comparison))
            {
                var proposedAlternative = symbolName == "frameworkElement"
                                          ? "element"
                                          : symbolName.WithoutSuffix("Element");

                yield return Issue(symbol, proposedAlternative);
            }

            foreach (var pair in WrongSuffixes)
            {
                if (symbolName.EndsWith(pair.Key, Comparison) && symbolName.StartsWithAny(Prefixes) is false)
                {
                    yield return Issue(symbol, pair.Value);
                }
            }
        }
    }
}