﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1030_BaseTypePrefixSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1030";

        public MiKo_1030_BaseTypePrefixSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static string FindBetterName(INamedTypeSymbol symbol) => symbol.Name.Without(Constants.Markers.BaseClasses);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var symbolName = symbol.Name.Without("Abstraction");

            foreach (var marker in Constants.Markers.BaseClasses)
            {
                if (symbolName.Contains(marker))
                {
                    yield return Issue(symbol, marker);
                }
            }
        }
    }
}