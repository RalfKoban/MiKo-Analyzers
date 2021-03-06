﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1037_EnumSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1037";

        private static readonly string[] WrongNames = { "Enums", "Enum" }; // order is important because of remove order

        public MiKo_1037_EnumSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static string FindBetterName(ITypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            var betterName = symbolName
                             .Replace("TypeEnum", "Kind")
                             .Without(WrongNames);

            if (betterName.IsNullOrWhiteSpace() is false)
            {
                if (symbol.IsEnum() && symbol.GetAttributeNames().Any(_ => _ == nameof(FlagsAttribute))
                                    && betterName.EndsWith("s", StringComparison.OrdinalIgnoreCase) is false)
                {
                    betterName = Pluralizer.GetPluralName(symbolName, betterName);
                }
            }

            return betterName;
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Name.EndsWithAny(WrongNames); // not only for enums, but also for other types (hence we do not use neither 'symbol.EnumUnderlyingType' nor 'symbol.IsEnum' here)

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var betterName = FindBetterName(symbol);

            return betterName.IsNullOrWhiteSpace()
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, betterName) };
        }
    }
}