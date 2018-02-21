using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1032_EnumSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1032";

        public MiKo_1032_EnumSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            // not only for enums, but also for other types (hence we do not use 'symbol.EnumUnderlyingType' here)
            if (!symbol.Name.EndsWithAny(StringComparison.OrdinalIgnoreCase, "Enum", "Enums")) return Enumerable.Empty<Diagnostic>();

            var betterName = symbol.Name
                                   .Replace("TypeEnums", "Kinds")
                                   .Replace("TypeEnum", "Kind")
                                   .Replace("Enums", string.Empty)
                                   .Replace("Enum", string.Empty);

            // ReSharper disable once RedundantNameQualifier we need the complete name here
            if (symbol.EnumUnderlyingType != null
                && symbol.GetAttributes().Any(_ => _.AttributeClass.Name == nameof(System.FlagsAttribute))
                && !betterName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                betterName = GetPluralName(betterName);
            }

            return new[] { ReportIssue(symbol, betterName) };

        }
    }
}