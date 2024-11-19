using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1037_TypeSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1037";

        private static readonly string[] WrongSuffixes =
                                                         {
                                                             // order is important here because of the order during removal
                                                             "Enums", "Enum",
                                                             "Types", "Type",
                                                             "Interfaces", "Interface",
                                                             "Classes", "Class",
                                                             "Structs", "Struct",
                                                             "Records", "Record",
                                                         };

        private static readonly string[] AllowedEnumSuffixes = { "Interfaces", "Interface", "Classes", "Class" };

        public MiKo_1037_TypeSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            if (symbol.IsEnum() && symbolName.EndsWithAny(AllowedEnumSuffixes, StringComparison.Ordinal))
            {
                return false;
            }

            return symbolName.EndsWithAny(WrongSuffixes);
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var betterName = FindBetterName(symbol);

            if (betterName.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }

        private static string FindBetterName(ITypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            var betterName = symbolName.AsBuilder()
                                       .ReplaceWithCheck("TypeEnum", "Kind")
                                       .Without(WrongSuffixes)
                                       .ToString();

            if (betterName.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            if (symbol.IsEnum() && betterName.EndsWith('s') is false && symbol.HasAttribute(Constants.Names.FlagsAttributeNames))
            {
                betterName = Pluralizer.GetPluralName(symbolName, betterName);
            }

            return betterName;
        }
    }
}