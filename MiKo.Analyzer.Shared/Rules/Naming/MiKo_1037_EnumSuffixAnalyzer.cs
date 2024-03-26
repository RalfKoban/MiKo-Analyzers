using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

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

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Name.EndsWithAny(WrongNames); // not only for enums, but also for other types (hence we do not use neither 'symbol.EnumUnderlyingType' nor 'symbol.IsEnum' here)

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var betterName = FindBetterName(symbol);

            if (betterName.IsNullOrWhiteSpace() is false)
            {
                yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
            }
        }

        private static string FindBetterName(ITypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            var betterName = new StringBuilder(symbolName).ReplaceWithCheck("TypeEnum", "Kind")
                                                          .Without(WrongNames)
                                                          .ToString();

            if (betterName.IsNullOrWhiteSpace() is false)
            {
                if (symbol.IsEnum() && betterName.EndsWith('s') is false && symbol.HasAttribute(Constants.Names.FlagsAttributeNames))
                {
                    betterName = Pluralizer.GetPluralName(symbolName, betterName);
                }
            }

            return betterName;
        }
    }
}