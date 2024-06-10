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

        protected override bool ShallAnalyze(IParameterSymbol symbol) => ShallAnalyze(symbol.GetEnclosingMethod());

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.Entity, Comparison))
            {
                var proposal = symbolName.WithoutSuffix(Constants.Entity);

                return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
            }

            if (symbolName.EndsWith("Element", Comparison))
            {
                var proposal = symbolName == "frameworkElement"
                               ? "element"
                               : symbolName.WithoutSuffix("Element");

                return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
            }

            return AnalyzeSuffixes();

            IEnumerable<Diagnostic> AnalyzeSuffixes()
            {
                foreach (var pair in WrongSuffixes)
                {
                    if (symbolName.EndsWith(pair.Key, Comparison) && symbolName.StartsWithAny(Prefixes) is false)
                    {
                        var proposal = pair.Value;

                        yield return Issue(symbol, proposal, CreateBetterNameProposal(proposal));
                    }
                }
            }
        }
    }
}