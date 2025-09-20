using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1516_ParametersWithAbilityNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1516";

        public MiKo_1516_ParametersWithAbilityNameAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol)
        {
            if (symbol.Type.IsBoolean())
            {
                var symbolName = GetPartToInspect(symbol.Name);

                return symbolName.EndsWith('y') && symbolName.HasUpperCaseLettersAbove(1) is false;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var betterName = AdjectiveFinder.GetAdjectiveForNoun(GetPartToInspect(symbol.Name), FirstWordAdjustment.StartLowerCase);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }

        private static ReadOnlySpan<char> GetPartToInspect(string name) => name.EndsWith("Condition", StringComparison.Ordinal)
                                                                           ? name.AsSpan(0, name.Length - 9)
                                                                           : name.AsSpan();
    }
}