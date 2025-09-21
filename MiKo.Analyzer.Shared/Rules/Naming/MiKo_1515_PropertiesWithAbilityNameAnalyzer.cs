using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1515_PropertiesWithAbilityNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1515";

        public MiKo_1515_PropertiesWithAbilityNameAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol)
        {
            if (symbol.Type.IsBoolean())
            {
                var symbolName = GetPartToInspect(symbol.Name);

                return symbolName.EndsWith('y') && symbolName.HasUpperCaseLettersAbove(1) is false;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation)
        {
            var betterName = AdjectiveFinder.GetAdjectiveForNoun(GetPartToInspect(symbol.Name));

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }

        private static ReadOnlySpan<char> GetPartToInspect(string name) => name.EndsWith("Condition", StringComparison.Ordinal)
                                                                           ? name.AsSpan(0, name.Length - 9)
                                                                           : name.AsSpan();
    }
}