using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1092_AbilityTypeWrongSuffixedAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1092";

        private const StringComparison Comparison = StringComparison.Ordinal;

        private static readonly string[] TypeSuffixes =
                                                        {
                                                            Constants.Element,
                                                            Constants.Entity,
                                                            "Item",
                                                            "Info",
                                                            "Information",
                                                        };

        public MiKo_1092_AbilityTypeWrongSuffixedAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Name.EndsWithAny(TypeSuffixes, Comparison);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var name = symbol.Name;

            if (name.Contains("able") && name.EndsWithAny(TypeSuffixes, Comparison))
            {
                var proposedName = GetProposedName(name);

                return new[] { Issue(symbol, proposedName, CreateBetterNameProposal(proposedName)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static string GetProposedName(string name)
        {
            var proposedName = name.AsSpan();

            do
            {
                foreach (var suffix in TypeSuffixes)
                {
                    if (proposedName.EndsWith(suffix, Comparison))
                    {
                        proposedName = proposedName.WithoutSuffix(suffix);
                    }
                }
            }
            while (proposedName.EndsWithAny(TypeSuffixes, Comparison));

            return proposedName.ToString();
        }
    }
}