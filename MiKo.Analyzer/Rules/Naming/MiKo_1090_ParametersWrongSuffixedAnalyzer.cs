using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1090_ParametersWrongSuffixedAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1090";

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

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol.ContainingSymbol is IMethodSymbol m && ShallAnalyze(m);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol)
        {
            // Ordinal is important here as we want to have only those with a suffix and not all (e.g. we want 'comparisonView' but not 'view')
            const StringComparison Comparison = StringComparison.Ordinal;

            var symbolName = symbol.Name;

            if (symbolName.EndsWith("Entity", Comparison))
            {
                return new[] { Issue(symbol, symbolName.WithoutSuffix("Entity")) };
            }
            else if (symbolName.EndsWith("Element", Comparison))
            {
                return new[] { Issue(symbol, symbolName.WithoutSuffix("Element")) };
            }

            foreach (var pair in WrongSuffixes)
            {
                if (symbolName.EndsWith(pair.Key, Comparison))
                {
                    return new[] { Issue(symbol, pair.Value) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}