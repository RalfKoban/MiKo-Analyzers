using System;
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

        public MiKo_1037_EnumSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Name.EndsWithAny(StringComparison.OrdinalIgnoreCase, "Enum", "Enums"); // not only for enums, but also for other types (hence we do not use neither 'symbol.EnumUnderlyingType' nor 'symbol.IsEnum' here)

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            var betterName = symbolName
                                   .Replace("TypeEnums", "Kinds")
                                   .Replace("TypeEnum", "Kind")
                                   .RemoveAll("Enums", "Enum");

            if (betterName.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            // ReSharper disable once RedundantNameQualifier we need the complete name here
            if (symbol.IsEnum()
                && symbol.GetAttributes().Any(_ => _.AttributeClass.Name == nameof(FlagsAttribute))
                && !betterName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                betterName = GetPluralName(symbolName, betterName);
            }

            return new[] { ReportIssue(symbol, betterName) };

        }
    }
}