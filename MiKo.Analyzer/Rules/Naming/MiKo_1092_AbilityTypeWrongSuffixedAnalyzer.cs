﻿using System;
using System.Collections.Generic;

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
                "Element",
                "Entity",
                "Item",
                "Info",
                "Information",
            };

        public MiKo_1092_AbilityTypeWrongSuffixedAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Name.EndsWithAny(TypeSuffixes, Comparison);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var name = symbol.Name;

            if (name.Contains("able"))
            {
                foreach (var suffix in TypeSuffixes)
                {
                    if (name.EndsWith(suffix, Comparison))
                    {
                        yield return Issue(symbol, name.WithoutSuffix(suffix));
                    }
                }
            }
        }
    }
}